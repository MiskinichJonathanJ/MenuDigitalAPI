import { showSuccess, showError, generateId } from '../utils/helpers.js';
import { appState } from '../store.js';

const CartService = {
    addToCart(dishId) {
        const dish = appState.dishes.find(d => d.id.toString() === dishId);
        if (!dish) {
            showError("Plato no encontrado");
            return;
        }

        const existing = appState.cart.find(item => item.dish.id === dish.id);
        if (existing) {
            existing.quantity++;
        } else {
            appState.cart.push({
                id: generateId(),
                dish,
                quantity: 1,
                notes: ""
            });
        }

        this.updateCartUI();
        showSuccess(`${dish.name} agregado al carrito`);
    },

    updateCartUI() {
        const container = document.getElementById('cart-items');
        const emptyCart = document.getElementById('empty-cart');
        if (!container) return;

        if (appState.cart.length > 0) {
            container.classList.remove('d-none');
            if (emptyCart) emptyCart.classList.add('d-none');
            container.innerHTML = appState.cart.map(item => `<li>${item.dish.name} x${item.quantity}</li>`).join('');
        } else {
            container.classList.add('d-none');
            if (emptyCart) emptyCart.classList.remove('d-none');
            container.innerHTML = '';
        }
    }
};
export { CartService };