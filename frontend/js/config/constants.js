export const DEFAULT_DISH_IMAGE = 'https://cdni.iconscout.com/illustration/premium/thumb/no-result-found-illustration-svg-download-png-11838290.png';

export const SELECTORS = {
    BRANDING_SCREEN: '#branding-screen',
    MENU_SECTION: '#menu-section',
    ORDERS_SECTION: '#orders-section',
    SHOW_MENU_BTN: '#show-menu-btn',
    SHOW_ORDERS_BTN: '#show-orders-btn',
    
    SEARCH_INPUT: '#search-input',
    SEARCH_INPUT_MOBILE: '#search-input-mobile',
    CATEGORY_BUTTONS: '.category-btn',
    CATEGORY_SELECT_MOBILE: '#category-select-mobile',
    PRICE_SORT: '#price-sort',
    PRICE_SORT_MOBILE: '#price-sort-mobile',
    
    DISHES_CONTAINER: '#dishes-container',
    NO_DISHES_MSG: '#no-dishes',
    DISH_MODAL: '#dishModal',
    DISH_MODAL_CONTENT: '#dish-modal-content',
    CATEGORIES_LIST: '#categories-list',
    CURRENT_CATEGORY: '#current-category',
    
    CART_OFFCANVAS: '#cartOffcanvas',
    CART_ITEMS: '#cart-items',
    CART_TOTAL: '#cart-total',
    CART_COUNT: '#cart-count',
    CART_FOOTER: '#cart-footer',
    CHECKOUT_BTN: '#checkout-btn',
    EMPTY_CART: '#empty-cart',
    
    ORDERS_CONTAINER: '#orders-container',
    
    FILTER_FROM: '#filter-from',
    FILTER_TO: '#filter-to',
    FILTER_STATUS: '#filter-status'
};

export const MESSAGES = {
    ADD_CART_SUCCESS: (name) => `${name} agregado al carrito`,
    REMOVE_CART_SUCCESS: (name) => `${name} eliminado del carrito`,
    ORDER_CREATED: (num) => `Orden #${num} creada exitosamente!`,
    ORDER_UPDATED: (num) => `Orden #${num} actualizada exitosamente!`,
    STATE_UPDATED: 'Estado actualizado correctamente',
    CART_CLEARED: 'Carrito vaciado',
    
    LOAD_DISHES_ERROR: 'Error al cargar platos',
    LOAD_CATEGORIES_ERROR: 'Error al cargar categorías',
    LOAD_ORDERS_ERROR: 'Error al cargar las órdenes',
    CREATE_ORDER_ERROR: 'Error al crear la orden. Intente nuevamente.',
    UPDATE_ORDER_ERROR: 'Error al actualizar la orden.',
    UPDATE_STATE_ERROR: 'Error al actualizar el estado del plato',
    INVALID_DATA: 'Datos inválidos',
    INVALID_QUANTITY: 'La cantidad debe ser un número entero mayor a 0',
    DISH_NOT_FOUND: 'Plato no encontrado',
    DISH_UNAVAILABLE: 'Este plato no está disponible en este momento',
    CART_ITEM_NOT_FOUND: 'Item no encontrado en el carrito',
    ORDER_NOT_FOUND: 'Orden no encontrada',
    CONNECTION_ERROR: 'Error de conexión. Por favor intente más tarde.',
    UNEXPECTED_ERROR: 'Ocurrió un error inesperado',
    ASYNC_ERROR: 'Error en operación asíncrona',
    CHECKOUT_ERROR: 'Error al procesar el checkout',
    DELIVERY_TYPES_ERROR: 'Error al cargar tipos de entrega',
    CATEGORY_INVALID: 'Categoría inválida',
};

export default {
    DEFAULT_DISH_IMAGE,
    SELECTORS,
    MESSAGES
};