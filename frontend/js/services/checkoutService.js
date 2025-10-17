import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { showSuccess, showError } from '../utils/helpers.js';
import { appStore } from '../appStore.js';
import { MESSAGES } from '../config/constants.js';

const CheckoutService = {
    async getDeliveryTypes() {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.DELIVERY_TYPES);
            return await apiRequest(url);
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
                body: JSON.stringify(orderData)
            });

            showSuccess(MESSAGES.ORDER_CREATED(response.orderNumber));
            return response;
        } catch (error) {
            showError(MESSAGES.CREATE_ORDER_ERROR);
            throw error;
        }
    },

    formatOrderRequest(deliveryType, deliveryAddress = '', notes = '') {
        const cart = appStore.getState('cart');

        return {
            items: cart.map(item => ({
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

export { CheckoutService };