import { formatPrice, escapeHtml } from '../utils/helpers.js';
import { isValidCartItem } from '../utils/validation.js';

export function renderCartItem(item) {
    if (!isValidCartItem(item)) return '';

    const subtotal = item.dish.price * item.quantity;
    const notesHTML = item.notes ? `
        <br>
        <small class="text-muted fst-italic">
            <i class="fas fa-sticky-note me-1" aria-hidden="true"></i>
            ${escapeHtml(item.notes)}
        </small>
    ` : '';

    return `
        <li class="cart-item mb-3 pb-3 border-bottom" data-cart-id="${item.id}">
            <div class="d-flex align-items-start mb-2">
                <img src="${item.dish.image || DEFAULT_DISH_IMAGE}" alt="${escapeHtml(item.dish.name)}" class="me-2 rounded">
                <div class="flex-grow-1">
                    <h6 class="mb-1">${escapeHtml(item.dish.name)}</h6>
                    <small class="text-muted">${formatPrice(item.dish.price)} c/u</small>
                    ${notesHTML}
                </div>
            </div>

            <div class="d-flex justify-content-between align-items-start mb-2">
                <div class="flex-grow-1">
                    <h6 class="mb-1">${escapeHtml(item.dish.name)}</h6>
                    <small class="text-muted">${formatPrice(item.dish.price)} c/u</small>
                    ${notesHTML}
                </div>
                <button class="btn btn-sm btn-outline-danger cart-remove-btn" 
                        data-cart-id="${item.id}"
                        aria-label="Eliminar ${escapeHtml(item.dish.name)}">
                    <i class="fas fa-trash" aria-hidden="true"></i>
                </button>
            </div>
            <div class="d-flex justify-content-between align-items-center">
                <div class="btn-group btn-group-sm" role="group">
                    <button class="btn btn-outline-secondary cart-decrease-btn" 
                            data-cart-id="${item.id}"
                            aria-label="Disminuir cantidad">
                        <i class="fas fa-minus" aria-hidden="true"></i>
                    </button>
                    <button class="btn btn-outline-secondary" disabled>
                        ${item.quantity}
                    </button>
                    <button class="btn btn-outline-secondary cart-increase-btn" 
                            data-cart-id="${item.id}"
                            aria-label="Aumentar cantidad">
                        <i class="fas fa-plus" aria-hidden="true"></i>
                    </button>
                </div>
                <strong>${formatPrice(subtotal)}</strong>
            </div>
        </li>
    `;
}

export function renderCartItems(items) {
    if (!Array.isArray(items)) return '';
    return items.map(item => renderCartItem(item)).filter(html => html !== '').join('');
}

export function updateCartUI(cartData) {
    const { items, total, totalItems } = cartData;

    const itemsContainer = document.getElementById('cart-items');
    const emptyMsg = document.getElementById('empty-cart');
    const footer = document.getElementById('cart-footer');
    const badge = document.getElementById('cart-count');

    if (!itemsContainer) return;

    if (items.length === 0) {
        itemsContainer.innerHTML = '';
        if (emptyMsg) emptyMsg.style.display = 'block';
        if (footer) footer.classList.add('d-none');
    } else {
        if (emptyMsg) emptyMsg.style.display = 'none';
        if (footer) footer.classList.remove('d-none');

        itemsContainer.innerHTML = renderCartItems(items);

        const totalElement = document.getElementById('cart-total');
        if (totalElement) {
            totalElement.textContent = total.toFixed(2);
        }
    }

    if (badge) {
        badge.textContent = String(totalItems);
        badge.classList.toggle('d-none', totalItems === 0);
    }
}