import { renderCheckoutForm, renderCheckoutSuccess } from './checkoutModal.js';
import { showError } from '../utils/helpers.js';
import { CheckoutService } from '../services/checkoutService.js';
import { CartService } from '../services/cartService.js';

export async function showCheckoutModal() {
    const cartData = CartService.getCartData();

    if (cartData.items.length === 0) {
        showError('El carrito está vacío');
        return;
    }

    const modalContent = document.getElementById('checkout-modal-content');
    if (!modalContent) {
        showError('Error al abrir checkout');
        return;
    }

    modalContent.innerHTML = '<div class="text-center py-5"><div class="spinner-border text-primary"></div></div>';

    try {
        const deliveryTypes = await CheckoutService.getDeliveryTypes();
        modalContent.innerHTML = renderCheckoutForm(cartData, deliveryTypes);

        setupCheckoutListeners();
    } catch (error) {
        modalContent.innerHTML = `
            <div class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                Error al cargar el formulario. Intente nuevamente.
            </div>
        `;
    }
    const modalEl = document.getElementById('checkoutModal');
    if (modalEl) {
        const modalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);
        modalInstance.show();
    }
}

function setupCheckoutListeners() {
    const form = document.getElementById('checkout-form');
    const deliveryTypeSelect = document.getElementById('delivery-type');
    const addressContainer = document.getElementById('address-container');

    if (deliveryTypeSelect) {
        deliveryTypeSelect.addEventListener('change', (e) => {
            const selectedOption = e.target.options[e.target.selectedIndex];
            const requiresAddress = selectedOption.text.toLowerCase().includes('delivery') ||
                selectedOption.text.toLowerCase().includes('domicilio');

            if (addressContainer) {
                addressContainer.style.display = requiresAddress ? 'block' : 'none';
                const addressInput = document.getElementById('delivery-address');
                if (addressInput) {
                    addressInput.required = requiresAddress;
                }
            }
        });
    }

    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            await handleCheckoutSubmit();
        });
    }
}

async function handleCheckoutSubmit() {
    const deliveryType = document.getElementById('delivery-type').value;
    const deliveryAddress = document.getElementById('delivery-address')?.value || '';
    const notes = document.getElementById('order-notes')?.value || '';

    if (!deliveryType) {
        showError('Debe seleccionar un tipo de entrega');
        return;
    }

    const modalContent = document.getElementById('checkout-modal-content');
    modalContent.innerHTML = '<div class="text-center py-5"><div class="spinner-border text-primary"></div><p class="mt-3">Procesando pedido...</p></div>';

    try {
        const orderData = CheckoutService.formatOrderRequest(deliveryType, deliveryAddress, notes);
        const response = await CheckoutService.createOrder(orderData);

        modalContent.innerHTML = renderCheckoutSuccess(response.orderNumber);

        CartService.clearCart();
    } catch (error) {
        modalContent.innerHTML = `
            <div class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                Error al procesar el pedido. Intente nuevamente.
            </div>
            <button class="btn btn-primary" onclick="location.reload()">Reintentar</button>
        `;
    }
}