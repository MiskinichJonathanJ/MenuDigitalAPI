import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { MESSAGES } from '../config/constants.js';
import { showSuccess, showError } from '../utils/helpers.js';

const OrderService = {
    async getDeliveryTypes() {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.DELIVERY_TYPES);
            const response = await apiRequest(url, { method: 'GET' });
            return response;
        } catch (error) {
            showError(MESSAGES.DELIVERY_TYPES_ERROR);
            throw error;
        }
    },

    async createOrder(orderData) {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.ORDERS);
            const response = await apiRequest(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(orderData)
            });

            showSuccess(MESSAGES.ORDER_CREATED(response.orderNumber));
            return response;
        } catch (error) {
            showError(MESSAGES.CREATE_ORDER_ERROR);
            throw error;
        }
    },

    async getAllOrders() {
        const url = buildApiUrl(API_CONFIG.ENDPOINTS.ORDERS);
        return await apiRequest(url, { method: 'GET' });
    },

    async getOrderById(orderId) {
        try {
            const url = buildApiUrl(`${API_CONFIG.ENDPOINTS.ORDERS}/${orderId}`);
            const response = await apiRequest(url, { method: 'GET' });
            return response;
        } catch (error) {
            showError(MESSAGES.CREATE_ORDER_ERROR);
            throw error;
        }
    },

    async updateOrder(orderNumber, updateData) {
        try {
            const url = buildApiUrl(`${API_CONFIG.ENDPOINTS.ORDERS}/${orderNumber}`);
            const response = await apiRequest(url, {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updateData)
            });
            return response;
        } catch (error) {
            showError(MESSAGES.UPDATE_ORDER_ERROR);
            throw error;
        }
    },

    formatOrderRequest(cartItems, deliveryType, deliveryAddress = '', notes = '') {
        return {
            items: cartItems.map(item => ({
                id: item.dish.id,
                quantity: item.quantity,
                notes: item.notes || ''
            })),
            delivery: {
                id: deliveryType,
                to: deliveryAddress
            },
            notes: notes
        };
    },

    async getOrders(filters = {}) {
        const queryParams = [];
        if (filters.from) queryParams.push(`from=${encodeURIComponent(filters.from)}`);
        if (filters.to) queryParams.push(`to=${encodeURIComponent(filters.to)}`);
        if (filters.status) queryParams.push(`status=${encodeURIComponent(filters.status)}`);

        const queryString = queryParams.length ? `?${queryParams.join('&')}` : '';
        const url = buildApiUrl(API_CONFIG.ENDPOINTS.ORDERS, queryString);

        return await apiRequest(url, { method: 'GET' });
    },

    async updateOrderItemStatus(orderNumber, itemId, newStatus) {
        try {
            const url = buildApiUrl(`${API_CONFIG.ENDPOINTS.ORDERS}/${orderNumber}/item/${itemId}`);
            const response = await apiRequest(url, {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ status: newStatus })
            });
            return response;
        } catch (error) {
            showError(MESSAGES.UPDATE_STATE_ERROR);
            throw error;
        }
    }
};

export { OrderService };
