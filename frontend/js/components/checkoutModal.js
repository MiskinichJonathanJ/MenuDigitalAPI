import { formatPrice, escapeHtml } from '../utils/helpers.js';

export function CheckoutModal() {
    return `
        <div class="modal fade" id="checkoutModal" tabindex="-1" aria-labelledby="checkoutModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="checkoutModalLabel">Finalizar Pedido</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                    </div>
                    <div class="modal-body" id="checkout-modal-content">
                        <div class="text-center py-5">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">Cargando...</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

export function renderCheckoutForm(cartData, deliveryTypes = []) {
    const itemsHTML = cartData.items.map(item => `
        <tr>
            <td>${escapeHtml(item.dish.name)}</td>
            <td class="text-center">${item.quantity}</td>
            <td class="text-end">${formatPrice(item.dish.price)}</td>
            <td class="text-end">${formatPrice(item.dish.price * item.quantity)}</td>
        </tr>
    `).join('');

    const deliveryTypesHTML = deliveryTypes.map(type => `
        <option value="${type.id}">${escapeHtml(type.name)}</option>
    `).join('');

    return `
        <div class="checkout-content">
            <h6 class="mb-3">Resumen del Pedido</h6>
            
            <div class="table-responsive mb-4">
                <table class="table table-sm">
                    <thead>
                        <tr>
                            <th>Plato</th>
                            <th class="text-center">Cant.</th>
                            <th class="text-end">Precio</th>
                            <th class="text-end">Subtotal</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${itemsHTML}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="3" class="text-end"><strong>Total:</strong></td>
                            <td class="text-end"><strong>${formatPrice(cartData.total)}</strong></td>
                        </tr>
                    </tfoot>
                </table>
            </div>

            <form id="checkout-form">
                <div class="mb-3">
                    <label for="delivery-type" class="form-label">Tipo de Entrega</label>
                    <select class="form-select" id="delivery-type" required>
                        <option value="">Seleccione...</option>
                        ${deliveryTypesHTML}
                    </select>
                </div>

                <div class="mb-3" id="address-container" style="display: none;">
                    <label for="delivery-address" class="form-label">Dirección de Entrega</label>
                    <input type="text" class="form-control" id="delivery-address" 
                           placeholder="Ej: Calle 123, Piso 4, Depto B">
                </div>

                <div class="mb-3">
                    <label for="order-notes" class="form-label">Notas adicionales (opcional)</label>
                    <textarea class="form-control" id="order-notes" rows="3"
                              placeholder="Ej: Sin timbre, llamar al llegar..."></textarea>
                </div>

                <div class="d-grid gap-2">
                    <button type="submit" class="btn btn-primary btn-lg">
                        <i class="fas fa-check me-2"></i>
                        Confirmar Pedido
                    </button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                        Cancelar
                    </button>
                </div>
            </form>
        </div>
    `;
}

export function renderCheckoutSuccess(orderNumber) {
    return `
        <div class="text-center py-5">
            <i class="fas fa-check-circle text-success mb-3" style="font-size: 4rem;"></i>
            <h4 class="mb-3">¡Pedido realizado con éxito!</h4>
            <p class="lead mb-4">Número de orden: <strong>#${orderNumber}</strong></p>
            <p class="text-muted">Recibirás una confirmación pronto.</p>
            <button class="btn btn-primary" data-bs-dismiss="modal">
                Cerrar
            </button>
        </div>
    `;
}