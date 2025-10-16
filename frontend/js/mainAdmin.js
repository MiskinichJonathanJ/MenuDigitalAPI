import { initAdminOrders } from './components/adminOrdersUI.js';

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => initAdminOrders('orders-container'));
} else {
    initAdminOrders('orders-container');
}
