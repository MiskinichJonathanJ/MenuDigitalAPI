import { OrderService } from '../services/orderService.js';
import { DishService } from '../services/dishService.js';
import { showError, showSuccess } from '../utils/helpers.js';

export async function renderOrders() {
    const container = document.getElementById('orders-container');
    if (!container) return;

    container.innerHTML = `<div class="text-center py-4">
        <div class="spinner-border" role="status"></div>
        <p class="text-muted mt-2">Cargando órdenes...</p>
    </div>`;

    try {
        const orders = await OrderService.getAllOrders();

        if (!orders || orders.length === 0) {
            container.innerHTML = `<div class="text-center py-5 text-muted">
                <i class="fas fa-receipt mb-3" style="font-size:2rem;"></i>
                <p>No hay órdenes registradas</p>
            </div>`;
            return;
        }

        container.innerHTML = orders.map(o => `
            <div class="card mb-3 p-3 bg-dark text-light">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h5 class="mb-0">Orden #${o.orderNumber}</h5>
                    <span class="badge bg-${getStatusColor(o.status)} text-uppercase">${o.status.name}</span>
                </div>
                <p class="mb-1 text-muted">Fecha: ${new Date(o.createdAt).toLocaleString()}</p>
                <p class="mb-1">Total: $${o.totalAmount.toFixed(2)}</p>
                <p class="mb-1 text-muted">Entrega: ${o.deliveryTo} (${o.deliveryType.name})</p>
                ${o.notes ? `<p class="mb-1 text-muted">Notas: ${o.notes}</p>` : ''}
                <button class="btn btn-sm btn-primary mt-2" data-order="${o.orderNumber}">
                    Ver detalles
                </button>
            </div>
        `).join('');

        container.querySelectorAll('button[data-order]').forEach(btn => {
            btn.addEventListener('click', () => {
                renderOrderDetails(btn.getAttribute('data-order'));
            });
        });

    } catch (err) {
        showError('Error al cargar las órdenes.');
        container.innerHTML = `<div class="text-center py-5 text-danger">
            <i class="fas fa-exclamation-triangle mb-3" style="font-size:2rem;"></i>
            <p>Error al cargar las órdenes.</p>
        </div>`;
    }
}

export async function renderOrderDetails(orderNumber) {
    const container = document.getElementById('orders-container');
    if (!container) return;

    container.innerHTML = `<div class="text-center py-4">
        <div class="spinner-border" role="status"></div>
        <p class="text-muted mt-2">Cargando orden #${orderNumber}...</p>
    </div>`;

    try {
        const activeDishes = await DishService.getAllDishes({ onlyActive: true });
        const order = await OrderService.getOrderById(orderNumber);

        container.innerHTML = `
            <button class="btn btn-outline-light mb-3" id="back-to-orders-btn">
                <i class="fas fa-arrow-left me-2"></i> Volver a órdenes
            </button>
            
            <div class="card p-3 bg-dark text-light">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h5 class="mb-0">Orden #${order.orderNumber}</h5>
                    <span class="badge bg-${getStatusColor(order.status)} text-uppercase">${order.status.name}</span>
                </div>
                <p class="mb-1 text-muted">Fecha: ${new Date(order.createdAt).toLocaleString()}</p>
                <p class="mb-1">Total: $${order.totalAmount.toFixed(2)}</p>
                <p class="mb-1 text-muted">Entrega: ${order.deliveryTo} (${order.deliveryType.name})</p>
                <hr class="border-secondary">
                <div id="order-items">
                    ${order.items.map(i => `
                        <div class="d-flex align-items-center mb-2" data-item-id="${i.dish.id}">
                            <img src="${i.dish.image}" alt="${i.dish.name}" class="me-2 rounded" style="width:40px; height:40px; object-fit:cover;">
                            <span class="me-2">${i.dish.name}</span>
                            <input type="number" min="1" value="${i.quantity}" class="form-control form-control-sm me-2" style="width:70px;" data-type="quantity">
                            <input type="text" value="${i.notes || ''}" class="form-control form-control-sm me-2" placeholder="Notas" data-type="notes">
                        </div>
                    `).join('')}
                </div>
                <div id="order-items-container" class="d-flex align-items-center mb-3 mt-3"></div>
                <button class="btn btn-success mt-3" id="save-order-btn">Guardar cambios</button>
            </div>
        `;

        // Filtrar platos activos que no estén en la orden
        const availableDishes = activeDishes.filter(d =>
            !order.items.some(item => item.dish.id === d.id)
        );

        const selectHTML = `
          <div id="add-dish-container" class="add-dish-container d-flex flex-wrap align-items-center gap-2 mt-3">
              <select id="new-dish-select" class="form-select form-select-sm flex-grow-1">
                  <option value="">Seleccionar plato...</option>
                  ${availableDishes.map(d => `<option value="${d.id}">${d.name}</option>`).join('')}
              </select>
              <input type="number" min="1" value="1"
                     class="form-control form-control-sm qty-input"
                     id="new-dish-qty" placeholder="Cant.">
              <input type="text"
                     class="form-control form-control-sm notes-input"
                     id="new-dish-notes" placeholder="Notas (opcional)">
              <button class="btn btn-sm btn-secondary flex-shrink-0" id="add-dish-btn">
                  Agregar
              </button>
          </div>
        `;

        document.getElementById('order-items-container').insertAdjacentHTML('beforeend', selectHTML);

        // Evento para agregar plato
        document.getElementById('add-dish-btn').addEventListener('click', () => {
            const select = document.getElementById('new-dish-select');
            const qty = parseInt(document.getElementById('new-dish-qty').value);
            const notes = document.getElementById('new-dish-notes').value;

            if (!select.value) return;

            const dish = availableDishes.find(d => d.id === select.value);

            const div = document.createElement('div');
            div.className = 'd-flex align-items-center mb-2';
            div.setAttribute('data-item-id', dish.id);
            div.innerHTML = `
                <img src="${dish.image}" alt="${dish.name}" class="me-2 rounded" style="width:40px; height:40px; object-fit:cover;">
                <span class="me-2">${dish.name}</span>
                <input type="number" min="1" value="${qty}" class="form-control form-control-sm me-2" style="width:70px;" data-type="quantity">
                <input type="text" value="${notes}" class="form-control form-control-sm me-2" placeholder="Notas" data-type="notes">
            `;
            document.getElementById('order-items').appendChild(div);

            // Actualizar select para quitar plato agregado
            const newAvailable = availableDishes.filter(d => d.id !== dish.id);
            const newOptions = newAvailable.map(d => `<option value="${d.id}">${d.name}</option>`).join('');
            select.innerHTML = `<option value="">Seleccionar plato...</option>${newOptions}`;
            document.getElementById('new-dish-qty').value = 1;
            document.getElementById('new-dish-notes').value = '';
        });
        document.getElementById('back-to-orders-btn').addEventListener('click', () => {
            renderOrders();
        });
        // Guardar cambios
        document.getElementById('save-order-btn').addEventListener('click', async () => {
            const updatedItems = Array.from(container.querySelectorAll('#order-items [data-item-id]')).map(div => ({
                id: div.getAttribute('data-item-id'),
                quantity: parseInt(div.querySelector('[data-type="quantity"]').value),
                notes: div.querySelector('[data-type="notes"]').value
            }));

            try {
                const updatedOrder = await OrderService.updateOrder(orderNumber, { items: updatedItems });
                showSuccess(`Orden #${updatedOrder.orderNumber} actualizada exitosamente!`);
                renderOrders();
            } catch (err) {
                showError(err.message || 'Error al actualizar la orden.');
            }
        });

    } catch (err) {
        showError('Error al cargar la orden.');
        container.innerHTML = `<div class="text-center py-5 text-danger"><p>Error al cargar la orden.</p></div>`;
    }
}

function getStatusColor(status) {
    const s = status.name.toLowerCase();
    switch (s) {
        case 'pendiente': return 'secondary';
        case 'confirmada': return 'info';
        case 'en preparación': return 'warning';
        case 'entregada': return 'success';
        case 'cancelada': return 'danger';
        default: return 'secondary';
    }
}
