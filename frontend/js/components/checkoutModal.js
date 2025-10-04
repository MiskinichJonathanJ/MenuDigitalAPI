export function CheckoutModal() {
    return `
        <div class="modal fade" id="checkoutModal" tabindex="-1" aria-labelledby="checkoutModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="checkoutModalLabel">
                            <i class="fas fa-shopping-bag me-2"></i>Confirmar Pedido
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <!-- Resumen del pedido -->
                        <div class="mb-4">
                            <h6 class="fw-bold mb-3">Resumen del Pedido</h6>
                            <div id="checkout-items-summary" class="mb-3">
                                <!-- Se llenará dinámicamente -->
                            </div>
                            <hr>
                            <div class="d-flex justify-content-between fw-bold">
                                <span>Total:</span>
                                <span id="checkout-total">$0.00</span>
                            </div>
                        </div>

                        <!-- Tipo de entrega -->
                        <div class="mb-3">
                            <label for="delivery-type-select" class="form-label">
                                <i class="fas fa-truck me-2"></i>Tipo de Entrega
                            </label>
                            <select class="form-select" id="delivery-type-select" required>
                                <option value="">Seleccione una opción...</option>
                            </select>
                        </div>

                        <!-- Dirección (solo para delivery) -->
                        <div class="mb-3 d-none" id="delivery-address-group">
                            <label for="delivery-address" class="form-label">
                                <i class="fas fa-map-marker-alt me-2"></i>Dirección de Entrega
                            </label>
                            <input 
                                type="text" 
                                class="form-control" 
                                id="delivery-address" 
                                placeholder="Ej: Av. Corrientes 1234, Buenos Aires"
                            >
                            <div class="invalid-feedback">
                                La dirección es obligatoria para delivery
                            </div>
                        </div>

                        <!-- Notas adicionales -->
                        <div class="mb-3">
                            <label for="order-notes" class="form-label">
                                <i class="fas fa-sticky-note me-2"></i>Notas Adicionales (Opcional)
                            </label>
                            <textarea 
                                class="form-control" 
                                id="order-notes" 
                                rows="3" 
                                placeholder="Ej: Timbre: Departamento 5B, Sin cebolla en las pizzas, etc."
                            ></textarea>
                            <small class="text-muted">Agrega cualquier instrucción especial para tu pedido</small>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>Cancelar
                        </button>
                        <button type="button" class="btn btn-primary" id="confirm-order-btn">
                            <i class="fas fa-check me-2"></i>Confirmar Pedido
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
}

export function renderCheckoutSummary(cartItems, formatPrice) {
    const summaryHTML = cartItems.map(item => `
        <div class="d-flex justify-content-between align-items-center mb-2">
            <div class="flex-grow-1">
                <span class="fw-medium">${item.dish.name}</span>
                <span class="text-muted"> x${item.quantity}</span>
                ${item.notes ? `<br><small class="text-muted fst-italic">${item.notes}</small>` : ''}
            </div>
            <span class="text-end">${formatPrice(item.dish.price * item.quantity)}</span>
        </div>
    `).join('');

    document.getElementById('checkout-items-summary').innerHTML = summaryHTML;
}

export function renderDeliveryTypes(deliveryTypes) {
    const select = document.getElementById('delivery-type-select');
    const optionsHTML = deliveryTypes.map(type =>
        `<option value="${type.id}">${type.name}</option>`
    ).join('');

    select.innerHTML = '<option value="">Seleccione una opción...</option>' + optionsHTML;
}