import { showSuccess, showError, generateId, formatPrice } from '../utils/helpers.js';
import { appState } from '../store.js';

const CartService = {
    save() {
        try { localStorage.setItem('cart', JSON.stringify(appState.cart || [])); }
        catch (e) { /* noop */ }
    },

    load() {
        try { appState.cart = JSON.parse(localStorage.getItem('cart')) || appState.cart || []; }
        catch (e) { appState.cart = appState.cart || []; }
    },

    findIndexByDishId(dishId) {
        return (appState.cart || []).findIndex(i => String(i.dish?.id) === String(dishId));
    },

    addToCart(payload) {
        const normalized = (typeof payload === 'string' || typeof payload === 'number')
            ? { id: String(payload), quantity: 1, notes: '' }
            : { id: String(payload.id), quantity: parseInt(payload.quantity || 1, 10), notes: String(payload.notes || '') };

        if (!normalized.id) { showError('ID de plato inválido'); return; }
        if (!Number.isInteger(normalized.quantity) || normalized.quantity < 1) { showError('Cantidad inválida'); return; }

        const dish = (appState.dishes || []).find(d => String(d.id) === String(normalized.id));
        if (!dish) { showError('Plato no encontrado'); return; }
        if (!dish.isActive) { showError('El plato no está disponible'); return; }

        appState.cart = appState.cart || [];

        const idx = this.findIndexByDishId(dish.id);
        if (idx >= 0) {
            appState.cart[idx].quantity += normalized.quantity;
            if (normalized.notes) {
                appState.cart[idx].notes = [appState.cart[idx].notes, normalized.notes].filter(Boolean).join(' | ');
            }
        } else {
            appState.cart.push({
                id: generateId(),
                dish,
                quantity: normalized.quantity,
                notes: normalized.notes || ''
            });
        }

        this.save();
        this.updateCartUI();
        document.dispatchEvent(new CustomEvent('cart:updated', { detail: appState.cart }));
        showSuccess(`${dish.name} agregado al carrito`);
    },

    updateItemQuantity(cartId, newQty) {
        if (!Number.isInteger(newQty) || newQty < 1) return;
        const idx = (appState.cart || []).findIndex(i => i.id === cartId);
        if (idx < 0) return;
        appState.cart[idx].quantity = newQty;
        this.save();
        this.updateCartUI();
        document.dispatchEvent(new CustomEvent('cart:updated', { detail: appState.cart }));
    },

    updateItemNotes(cartId, notes) {
        const idx = (appState.cart || []).findIndex(i => i.id === cartId);
        if (idx < 0) return;
        appState.cart[idx].notes = String(notes || '');
        this.save();
        this.updateCartUI();
        document.dispatchEvent(new CustomEvent('cart:updated', { detail: appState.cart }));
    },

    removeItem(cartId) {
        const idx = (appState.cart || []).findIndex(i => i.id === cartId);
        if (idx < 0) return;
        appState.cart.splice(idx, 1);
        this.save();
        this.updateCartUI();
        document.dispatchEvent(new CustomEvent('cart:updated', { detail: appState.cart }));
    },

    updateCartUI() {
        const itemsContainer = document.getElementById('cart-items');
        const emptyMsg = document.getElementById('empty-cart');
        const footer = document.getElementById('cart-footer');

        if (!itemsContainer || !emptyMsg || !footer) return;

        if (appState.cart.length === 0) {
            itemsContainer.innerHTML = '';
            emptyMsg.style.display = 'block';
            footer.classList.add('d-none');
        } else {
            emptyMsg.style.display = 'none';
            footer.classList.remove('d-none');

            // Renderizar items del carrito con mejor diseño
            itemsContainer.innerHTML = appState.cart.map(item => `
                <li class="cart-item mb-3 pb-3 border-bottom">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <div class="flex-grow-1">
                            <h6 class="mb-1">${item.dish.name}</h6>
                            <small class="text-muted">${formatPrice(item.dish.price)} c/u</small>
                            ${item.notes ? `<br><small class="text-muted fst-italic"><i class="fas fa-sticky-note me-1"></i>${item.notes}</small>` : ''}
                        </div>
                        <button class="btn btn-sm btn-outline-danger" onclick="window.CartService.removeItem('${item.id}')">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                    <div class="d-flex justify-content-between align-items-center">
                        <div class="btn-group btn-group-sm" role="group">
                            <button class="btn btn-outline-secondary" onclick="window.CartService.updateItemQuantity('${item.id}', ${item.quantity - 1})">
                                <i class="fas fa-minus"></i>
                            </button>
                            <button class="btn btn-outline-secondary" disabled>
                                ${item.quantity}
                            </button>
                            <button class="btn btn-outline-secondary" onclick="window.CartService.updateItemQuantity('${item.id}', ${item.quantity + 1})">
                                <i class="fas fa-plus"></i>
                            </button>
                        </div>
                        <strong>${formatPrice(item.dish.price * item.quantity)}</strong>
                    </div>
                </li>
            `).join('');

            const total = appState.cart.reduce((sum, i) => sum + i.dish.price * i.quantity, 0);
            document.getElementById('cart-total').textContent = total.toFixed(2);
        }

        this.updateBadge();
    },

    getTotalItems() {
        return (appState.cart || []).reduce((sum, it) => sum + (Number(it.quantity) || 0), 0);
    },

    updateBadge() {
        const badge = document.getElementById('cart-count');
        if (!badge) return;
        const total = this.getTotalItems();
        badge.textContent = String(total);
        badge.classList.toggle('d-none', total === 0);
    },
};

// Exponer CartService globalmente para los onclick en el HTML
window.CartService = CartService;

// Inicializar carga
CartService.load();
CartService.updateCartUI();
CartService.updateBadge();

export { CartService };