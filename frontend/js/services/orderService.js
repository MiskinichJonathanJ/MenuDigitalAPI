import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { showSuccess, showError } from '../utils/helpers.js';

const OrderService = {
    async getDeliveryTypes() {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.DELIVERY_TYPES);
            const response = await apiRequest(url, { method: 'GET' });
            return response;
        } catch (error) {
            showError('Error al cargar tipos de entrega');
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

            showSuccess(`Orden #${response.orderNumber} creada exitosamente!`);
            return response;
        } catch (error) {
            showError('Error al crear la orden. Intente nuevamente.');
            throw error;
        }
    },

    async getOrderById(orderId) {
        try {
            const url = buildApiUrl(`${API_CONFIG.ENDPOINTS.ORDERS}/${orderId}`);
            const response = await apiRequest(url, { method: 'GET' });
            return response;
        } catch (error) {
            showError('Error al obtener la orden');
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
    }
};

export { OrderService };