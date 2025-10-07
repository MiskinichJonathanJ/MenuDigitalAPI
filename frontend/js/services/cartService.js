import { showSuccess, showError, generateId, showMessage } from '../utils/helpers.js';
import { isValidQuantity } from '../utils/validation.js';
import { appStore } from '../appStore.js';
import { updateCartUI } from '../components/cartUI.js';

let cartMemoryStorage = [];

const CartService = {
    load() {
        try {
            const cart = cartMemoryStorage || [];
            appStore.setState('cart', cart);
        } catch (error) {
            appStore.setState('cart', []);
        }
    },

    save() {
        try {
            const cart = appStore.getState('cart');
            cartMemoryStorage = [...cart];
        } catch (error) {
        }
    },

    findIndexByDishId(dishId) {
        const cart = appStore.getState('cart');
        return cart.findIndex(item => String(item.dish?.id) === String(dishId));
    },

    findIndexByCartId(cartItemId) {
        const cart = appStore.getState('cart');
        return cart.findIndex(item => item.id === cartItemId);
    },

    addToCart(payload) {
        let normalized;

        if (typeof payload === 'string' || typeof payload === 'number') {
            normalized = {
                dishId: String(payload),
                quantity: 1,
                notes: ''
            };
        } else if (payload && typeof payload === 'object') {
            normalized = {
                dishId: String(payload.id || payload.dishId),
                quantity: parseInt(payload.quantity, 10) || 1,
                notes: String(payload.notes || '').trim()
            };
        } else {
            showError('Datos inválidos para agregar al carrito');
            return false;
        }

        if (!isValidQuantity(normalized.quantity)) {
            showError('La cantidad debe ser un número entero mayor a 0');
            return false;
        }

        const dishes = appStore.getState('dishes');
        const dish = dishes.find(d => String(d.id) === normalized.dishId);

        if (!dish) {
            showError('Plato no encontrado');
            return false;
        }

        if (!dish.isActive) {
            showError('Este plato no está disponible en este momento');
            return false;
        }

        const cart = [...appStore.getState('cart')];
        const existingIndex = this.findIndexByDishId(dish.id);

        if (existingIndex >= 0) {
            cart[existingIndex].quantity += normalized.quantity;

            if (normalized.notes) {
                const existingNotes = cart[existingIndex].notes || '';
                cart[existingIndex].notes = existingNotes
                    ? `${existingNotes} | ${normalized.notes}`
                    : normalized.notes;
            }
        } else {
            cart.push({
                id: generateId(),
                dish: { ...dish },
                quantity: normalized.quantity,
                notes: normalized.notes
            });
        }

        appStore.setState('cart', cart);
        showSuccess(`${dish.name} agregado al carrito`);
        return true;
    },

    updateItemQuantity(cartItemId, newQuantity) {
        if (!isValidQuantity(newQuantity)) {
            showMessage('La cantidad debe ser un número entero mayor a 0', 'warning');
            return false;
        }

        const cart = [...appStore.getState('cart')];
        const index = this.findIndexByCartId(cartItemId);

        if (index < 0) {
            showError('Item no encontrado en el carrito');
            return false;
        }

        cart[index].quantity = newQuantity;
        appStore.setState('cart', cart);
        return true;
    },

    updateItemNotes(cartItemId, notes) {
        const cart = [...appStore.getState('cart')];
        const index = this.findIndexByCartId(cartItemId);

        if (index < 0) {
            showError('Item no encontrado en el carrito');
            return false;
        }

        cart[index].notes = String(notes || '').trim();
        appStore.setState('cart', cart);
        return true;
    },

    removeItem(cartItemId) {
        const cart = [...appStore.getState('cart')];
        const index = this.findIndexByCartId(cartItemId);

        if (index < 0) {
            showError('Item no encontrado en el carrito');
            return false;
        }

        const itemName = cart[index].dish.name;
        cart.splice(index, 1);

        appStore.setState('cart', cart);
        showSuccess(`${itemName} eliminado del carrito`);
        return true;
    },

    clearCart() {
        appStore.setState('cart', []);
        showSuccess('Carrito vaciado');
    },

    getTotal() {
        const cart = appStore.getState('cart');
        return cart.reduce((sum, item) => sum + (item.dish.price * item.quantity), 0);
    },

    getTotalItems() {
        const cart = appStore.getState('cart');
        return cart.reduce((sum, item) => sum + item.quantity, 0);
    },

    getCartData() {
        return {
            items: appStore.getState('cart'),
            total: this.getTotal(),
            totalItems: this.getTotalItems()
        };
    },

    setupEventListeners() {
        document.addEventListener('click', (e) => {
            const target = e.target.closest('button');
            if (!target) return;

            const cartId = target.dataset.cartId;
            if (!cartId) return;

            if (target.classList.contains('cart-remove-btn')) {
                this.removeItem(cartId);
            }
            else if (target.classList.contains('cart-decrease-btn')) {
                const cart = appStore.getState('cart');
                const item = cart.find(i => i.id === cartId);
                if (item && item.quantity > 1) {
                    this.updateItemQuantity(cartId, item.quantity - 1);
                } else {
                    this.removeItem(cartId);
                }
            }
            else if (target.classList.contains('cart-increase-btn')) {
                const cart = appStore.getState('cart');
                const item = cart.find(i => i.id === cartId);
                if (item) {
                    this.updateItemQuantity(cartId, item.quantity + 1);
                }
            }
        });
    },

    init() {
        this.load();

        appStore.subscribe('cart', () => {
            this.save();
            updateCartUI(this.getCartData());
        });

        this.setupEventListeners();
        updateCartUI(this.getCartData());
    },
};

export { CartService };