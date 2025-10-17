import { MESSAGES, SELECTORS } from '../config/constants.js';
import { OrderService } from '../services/orderService.js';
import { StatusService } from '../services/statusService.js';
import { showSuccess, showError } from '../utils/helpers.js';

let statusMap = {}; 

async function initAdminOrders(containerId = SELECTORS.ORDERS_CONTAINER) {
    try {
        statusMap = await StatusService.getStatusMap();
        await loadOrders(containerId);
        setupFilters(containerId);
    } catch (error) {
        showError(MESSAGES.LOAD_ORDERS_ERROR);
    }
}

async function loadOrders(containerId, filters = {}) {
    try {
        let orders = Object.keys(filters).length
            ? await OrderService.getOrders(filters)
            : await OrderService.getAllOrders();

        renderOrders(containerId, orders);
    } catch (error) {
        console.error(error);
        showError(MESSAGES.LOAD_ORDERS_ERROR);
    }
}

export function renderOrders(containerId, orders) {
    const container = document.getElementById(containerId);
    if (!container) return;

    container.innerHTML = '';

    if (!orders || orders.length === 0) {
        container.innerHTML = `<p class="empty-msg">No hay órdenes para mostrar</p>`;
        return;
    }

    orders.forEach(order => {
        const orderCard = document.createElement('div');
        orderCard.className = 'order-card shadow-sm p-3 mb-3 rounded';

        const itemsHtml = order.items.map(item => `
            <div class="order-item d-flex justify-content-between align-items-center p-2 mb-1 rounded bg-light">
                <div>
                    <p class="mb-0 fw-bold">${item.dish.name} x${item.quantity}</p>
                    ${item.notes ? `<p class="mb-0 text-muted">Notas: ${item.notes}</p>` : ''}
                </div>
                <select class="form-select w-auto" data-order-id="${order.orderNumber}" data-item-id="${item.id}">
                    ${renderStatusOptions(item.status.id)}
                </select>
            </div>
        `).join('');

        orderCard.innerHTML = `
            <div class="order-header d-flex justify-content-between align-items-center mb-2">
                <div>
                    <h5 class="mb-0">Orden #${order.orderNumber}</h5>
                    <small class="text-muted">Fecha: ${new Date(order.createdAt).toLocaleString()}</small>
                </div>
                <span class="badge bg-primary text-white">Estado: ${order.status.name}</span>
            </div>

            <div class="order-info mb-2">
                <p class="mb-1"><i class="fas fa-truck me-2"></i>Entrega: ${order.deliveryType.name} - ${order.deliveryTo}</p>
                ${order.notes ? `<p class="mb-0 text-muted">Notas: ${order.notes}</p>` : ''}
            </div>

            <div class="order-items">
                ${itemsHtml}
            </div>
        `;

        container.appendChild(orderCard);
    });

    container.querySelectorAll('select.form-select').forEach(select => {
        select.addEventListener('change', async (e) => {
            const orderId = e.target.dataset.orderId;
            const itemId = e.target.dataset.itemId;
            const newStatus = e.target.value;

            try {
                await OrderService.updateOrderItemStatus(orderId, itemId, newStatus);
                showSuccess(MESSAGES.STATE_UPDATED);
                await loadOrders(containerId);
            } catch (error) {
                console.error(error);
                showError(MESSAGES.UPDATE_STATE_ERROR);
            }
        });
    });
}

function renderStatusOptions(currentId) {
    return Object.entries(statusMap)
        .map(([id, name]) => `<option value="${id}" ${id == currentId ? 'selected' : ''}>${name}</option>`)
        .join('');
}


function setupFilters(containerId) {
    const filterBtn = document.getElementById('filter-orders-btn');
    const statusSelect = document.getElementById('filter-status');

    if (statusSelect) {
        statusSelect.innerHTML = '<option value="">Todos</option>' +
            Object.entries(statusMap)
                .map(([id, name]) => `<option value="${id}">${name}</option>`)
                .join('');
    }

    filterBtn?.addEventListener('click', async () => {
        const fromInput = document.getElementById(SELECTORS.FILTER_FROM).value;
        const toInput = document.getElementById(SELECTORS.FILTER_TO).value;

        const fromDate = fromInput ? new Date(fromInput) : null;
        const toDate = toInput ? new Date(toInput) : null;

        const filters = {
            from: fromDate ? fromDate.toISOString() : undefined,
            to: toDate ? toDate.toISOString() : undefined,
            status: document.getElementById(SELECTORS.FILTER_STATUS).value
        };

        await loadOrders(containerId, filters);
    });
}

export { initAdminOrders };