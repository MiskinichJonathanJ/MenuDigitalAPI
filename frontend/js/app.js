import { showError, showSuccess, showMessage, debounce } from './utils/helpers.js';
import { CategoryService } from './services/categoryService.js';
import { DishService } from './services/dishService.js';
import { CartService } from './services/cartService.js';
import { CheckoutModal } from './components/checkoutModal.js';
import { showCheckoutModal } from './components/checkoutUI.js';
import { appStore } from './appStore.js';
import { updateCartUI } from './components/cartUI.js';

async function initApp() {
    try {
        showMessage('Cargando aplicación...', 'info');

        if (!document.getElementById('checkoutModal')) {
            document.body.insertAdjacentHTML('beforeend', CheckoutModal());
        }

        CartService.init();
        await CategoryService.init();
        await DishService.init();

        showSuccess('¡Aplicación lista!');
    } catch (error) {
        showError(`Error al cargar la aplicación: ${error.message}`);
        throw error;
    }
}

function setupEventListeners() {
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            const openModal = document.querySelector('.modal.show');
            if (openModal) {
                const bsModal = bootstrap.Modal.getInstance(openModal);
                if (bsModal) bsModal.hide();
            }
        }
    });

    const cartOffcanvas = document.getElementById('cartOffcanvas');
    if (cartOffcanvas) {
        cartOffcanvas.addEventListener('show.bs.offcanvas', () => {
            updateCartUI(CartService.getCartData());
        });
    }

    const checkoutBtn = document.getElementById('checkout-btn');
    if (checkoutBtn) {
        checkoutBtn.addEventListener('click', async () => {
            await showCheckoutModal();
        });
    }

    setupFilterListeners();
}

function setupFilterListeners() {
    const searchInputDesktop = document.getElementById('search-input');
    const searchInputMobile = document.getElementById('search-input-mobile');

    const onlyAvailableDesktop = document.getElementById('only-available');
    const onlyAvailableMobile = document.getElementById('only-available-mobile');

    const priceSortDesktop = document.getElementById('price-sort');
    const priceSortMobile = document.getElementById('price-sort-mobile');

    const debouncedSearch = debounce((value) => {
        appStore.setState('searchTerm', value);
    }, 300);

    if (searchInputDesktop) {
        searchInputDesktop.addEventListener('input', (e) => {
            const value = e.target.value;
            if (searchInputMobile) searchInputMobile.value = value;
            debouncedSearch(value);
        });
    }

    if (searchInputMobile) {
        searchInputMobile.addEventListener('input', (e) => {
            const value = e.target.value;
            if (searchInputDesktop) searchInputDesktop.value = value;
            debouncedSearch(value);
        });
    }

    if (onlyAvailableDesktop) {
        onlyAvailableDesktop.addEventListener('change', (e) => {
            const checked = e.target.checked;
            if (onlyAvailableMobile) onlyAvailableMobile.checked = checked;

            const currentFilters = appStore.getState('filters');
            appStore.setState('filters', {
                ...currentFilters,
                onlyAvailable: checked
            });
        });
    }

    if (onlyAvailableMobile) {
        onlyAvailableMobile.addEventListener('change', (e) => {
            const checked = e.target.checked;
            if (onlyAvailableDesktop) onlyAvailableDesktop.checked = checked;

            const currentFilters = appStore.getState('filters');
            appStore.setState('filters', {
                ...currentFilters,
                onlyAvailable: checked
            });
        });
    }

    if (priceSortDesktop) {
        priceSortDesktop.addEventListener('change', (e) => {
            const value = e.target.value;
            if (priceSortMobile) priceSortMobile.value = value;

            const currentFilters = appStore.getState('filters');
            appStore.setState('filters', {
                ...currentFilters,
                sortByPrice: value
            });
        });
    }

    if (priceSortMobile) {
        priceSortMobile.addEventListener('change', (e) => {
            const value = e.target.value;
            if (priceSortDesktop) priceSortDesktop.value = value;

            const currentFilters = appStore.getState('filters');
            appStore.setState('filters', {
                ...currentFilters,
                sortByPrice: value
            });
        });
    }
}

function setupErrorHandling() {
    window.addEventListener('error', (event) => {
        showError('Ocurrió un error inesperado');
    });

    window.addEventListener('unhandledrejection', (event) => {
        showError('Error en operación asíncrona');
    });
}

export { initApp, setupEventListeners, setupErrorHandling };