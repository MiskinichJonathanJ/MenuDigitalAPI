console.log('orderService.js cargado');

const OrderService = {
    async createOrder(orderData) {
        try {
            console.log('Creando orden:', orderData);
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.ORDERS);
            const response = await apiRequest(url, {
                method: 'POST',
                body: JSON.stringify(orderData)
            });

            console.log('Orden creada:', response);
            return response;
        } catch (error) {
            console.error('Error al crear orden:', error);
            throw error;
        }
    }
};

window.OrderService = OrderService;