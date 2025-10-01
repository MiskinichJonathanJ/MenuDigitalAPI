console.log('cart.js cargado');

const CartService = {
    updateCartUI() {
        console.log('Actualizando UI del carrito...');
        const cart = window.appState.cart || [];

        // Actualizar contador
        const cartCount = document.getElementById('cart-count');
        if (cartCount) {
            const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
            cartCount.textContent = totalItems;
        }

        // Actualizar contenido del carrito
        const emptyCart = document.getElementById('empty-cart');
        const cartItems = document.getElementById('cart-items');
        const cartFooter = document.getElementById('cart-footer');

        if (cart.length === 0) {
            if (emptyCart) emptyCart.classList.remove('d-none');
            if (cartItems) cartItems.classList.add('d-none');
            if (cartFooter) cartFooter.classList.add('d-none');
        } else {
            if (emptyCart) emptyCart.classList.add('d-none');
            if (cartItems) cartItems.classList.remove('d-none');
            if (cartFooter) cartFooter.classList.remove('d-none');

            this.renderCartItems(cart);
            this.updateTotal(cart);
        }
    },

    renderCartItems(cart) {
        const cartItems = document.getElementById('cart-items');
        if (!cartItems) return;

        cartItems.innerHTML = cart.map(item => `
        <div class="cart-item d-flex align-items-center mb-2" data-item-id="${item.id}">
            <img src="${item.dish.image || "none"}" 
                 class="cart-item-img" alt="${item.dish.name}">
            <div class="cart-item-details">
              <div class="cart-item-name">${item.dish.name}</div>
              <div class="cart-item-price">${window.formatPrice(item.dish.price)}</div>
            </div>
            <div class="cart-item-controls">
              <div class="quantity-control">
                <button class="quantity-btn" onclick="window.CartService.decreaseQuantity('${item.id}')">-</button>
                <span class="quantity-input">${item.quantity}</span>
                <button class="quantity-btn" onclick="window.CartService.increaseQuantity('${item.id}')">+</button>
              </div>
              <button class="remove-item-btn" onclick="window.CartService.removeItem('${item.id}')">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
    `).join('');
    },

    updateTotal(cart) {
        const cartTotal = document.getElementById('cart-total');
        if (!cartTotal) return;

        const total = cart.reduce((sum, item) => sum + (item.dish.price * item.quantity), 0);
        cartTotal.textContent = total.toFixed(2);
    },

    increaseQuantity(itemId) {
        const item = window.appState.cart.find(i => i.id === itemId);
        if (item) {
            item.quantity++;
            this.updateCartUI();
        }
    },

    decreaseQuantity(itemId) {
        const item = window.appState.cart.find(i => i.id === itemId);
        if (item && item.quantity > 1) {
            item.quantity--;
            this.updateCartUI();
        }
    },

    removeItem(itemId) {
        window.appState.cart = window.appState.cart.filter(i => i.id !== itemId);
        this.updateCartUI();
        if (window.showSuccess) {
            window.showSuccess('Plato eliminado del carrito');
        }
    }
};

window.CartService = CartService;