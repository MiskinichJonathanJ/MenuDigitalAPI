import { OrderService } from './orderService.js';
import { renderCheckoutSummary, renderDeliveryTypes } from '../components/checkoutModal.js';
import { formatPrice } from '../utils/helpers.js';
import { appState } from '../store.js';

const CheckoutController = {
    deliveryTypes: [],

    async init() {
        try {
            this.deliveryTypes = await OrderService.getDeliveryTypes();
            this.setupEventListeners();
        } catch (error) {
            console.error('Error inicializando checkout:', error);
        }
    },

    setupEventListeners() {
        // Listener para el botón "Realizar Pedido" del offcanvas
        const checkoutBtn = document.getElementById('checkout-btn');
        if (checkoutBtn) {
            checkoutBtn.addEventListener('click', () => {
                const cartItems = appState.cart || [];

                if (cartItems.length === 0) {
                    alert('El carrito está vacío');
                    return;
                }

                const total = cartItems.reduce((sum, item) => sum + (item.dish.price * item.quantity), 0);

                // Cerrar el offcanvas del carrito
                const offcanvasElement = document.getElementById('cartOffcanvas');
                const offcanvasInstance = bootstrap.Offcanvas.getInstance(offcanvasElement);
                if (offcanvasInstance) {
                    offcanvasInstance.hide();
                }

                // Abrir modal de checkout
                this.openCheckout(cartItems, total);
            });
        }

        // Listener para cambio de tipo de entrega
        const deliverySelect = document.getElementById('delivery-type-select');
        const addressGroup = document.getElementById('delivery-address-group');
        const addressInput = document.getElementById('delivery-address');

        if (deliverySelect) {
            deliverySelect.addEventListener('change', (e) => {
                const selectedType = this.deliveryTypes.find(t => t.id == e.target.value);

                // Si el tipo es "Delivery", mostrar dirección
                if (selectedType && selectedType.name.toLowerCase().includes('delivery')) {
                    addressGroup.classList.remove('d-none');
                    addressInput.setAttribute('required', 'required');
                } else {
                    addressGroup.classList.add('d-none');
                    addressInput.removeAttribute('required');
                    addressInput.value = '';
                }
            });
        }

        // Listener para confirmar orden
        const confirmBtn = document.getElementById('confirm-order-btn');
        if (confirmBtn) {
            confirmBtn.addEventListener('click', () => this.handleConfirmOrder());
        }
    },

    openCheckout(cartItems, total) {
        // Renderizar tipos de entrega
        renderDeliveryTypes(this.deliveryTypes);

        // Renderizar resumen
        renderCheckoutSummary(cartItems, formatPrice);
        document.getElementById('checkout-total').textContent = formatPrice(total);

        // Limpiar formulario
        document.getElementById('delivery-type-select').value = '';
        document.getElementById('delivery-address').value = '';
        document.getElementById('order-notes').value = '';
        document.getElementById('delivery-address-group').classList.add('d-none');

        // Abrir modal
        const modal = new bootstrap.Modal(document.getElementById('checkoutModal'));
        modal.show();
    },

    async handleConfirmOrder() {
        const deliverySelect = document.getElementById('delivery-type-select');
        const addressInput = document.getElementById('delivery-address');
        const notesInput = document.getElementById('order-notes');
        const confirmBtn = document.getElementById('confirm-order-btn');

        // Validar tipo de entrega
        if (!deliverySelect.value) {
            deliverySelect.classList.add('is-invalid');
            return;
        }
        deliverySelect.classList.remove('is-invalid');

        // Validar dirección si es delivery
        const selectedType = this.deliveryTypes.find(t => t.id == deliverySelect.value);
        if (selectedType && selectedType.name.toLowerCase().includes('delivery')) {
            if (!addressInput.value.trim()) {
                addressInput.classList.add('is-invalid');
                return;
            }
            addressInput.classList.remove('is-invalid');
        }

        // Deshabilitar botón mientras se procesa
        confirmBtn.disabled = true;
        confirmBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Procesando...';

        try {
            // Obtener items del carrito desde appState
            const cartItems = appState.cart || [];

            if (cartItems.length === 0) {
                throw new Error('El carrito está vacío');
            }

            // Formatear datos de la orden
            const orderData = OrderService.formatOrderRequest(
                cartItems,
                parseInt(deliverySelect.value),
                addressInput.value.trim(),
                notesInput.value.trim()
            );

            console.log('Enviando orden:', orderData);

            // Crear orden
            const response = await OrderService.createOrder(orderData);

            // Cerrar modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('checkoutModal'));
            modal.hide();

            // Limpiar carrito
            this.clearCart();

            // Mostrar confirmación
            this.showOrderConfirmation(response.orderNumber);

        } catch (error) {
            console.error('Error al confirmar orden:', error);
            alert('Error al crear la orden: ' + error.message);
        } finally {
            // Rehabilitar botón
            confirmBtn.disabled = false;
            confirmBtn.innerHTML = '<i class="fas fa-check me-2"></i>Confirmar Pedido';
        }
    },

    clearCart() {
        appState.cart = [];

        // Guardar en localStorage si está disponible
        try {
            localStorage.setItem('cart', JSON.stringify([]));
        } catch (e) {
            // Ignorar si falla
        }

        // Disparar evento para actualizar UI del carrito
        document.dispatchEvent(new CustomEvent('cart:updated'));
    },

    showOrderConfirmation(orderNumber) {
        const confirmHTML = `
            <div class="alert alert-success alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" role="alert" style="z-index: 9999; min-width: 300px;">
                <h4 class="alert-heading"><i class="fas fa-check-circle me-2"></i>¡Pedido Confirmado!</h4>
                <p class="mb-0">Tu orden #${orderNumber} ha sido creada exitosamente.</p>
                <hr>
                <p class="mb-0 small">Recibirás una notificación cuando tu pedido esté listo.</p>
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;

        document.body.insertAdjacentHTML('afterbegin', confirmHTML);

        // Auto-cerrar después de 5 segundos
        setTimeout(() => {
            const alert = document.querySelector('.alert-success');
            if (alert) {
                alert.remove();
            }
        }, 5000);
    }
};

export { CheckoutController };