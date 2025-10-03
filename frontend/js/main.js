import { initApp, setupEventListeners } from './app.js';
import { CartService } from './services/cartService.js';
import { DishService } from './services/dishService.js';
import { appState } from './store.js';

function hideLoader() {
    const loader = document.getElementById('loader');
    if (loader) {
        loader.classList.add('fade-out');
        setTimeout(() => {
            loader.style.display = 'none';
        }, 300);
    }
}
document.addEventListener('DOMContentLoaded', () => {
    CartService.load();
    CartService.updateCartUI();
    CartService.updateBadge();

    const cartModalEl = document.getElementById('cartModal');
    if (cartModalEl) {
        cartModalEl.addEventListener('show.bs.modal', () => {
            CartService.updateCartUI();
            CartService.updateBadge();
        });
    }
    setupEventListeners();
    initApp();
    function debounce(fn, wait = 250) {
        let t;
        return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), wait); };
    }

    // lee los valores actuales de los controles y actualiza appState
    function updateAppStateFromControl(filterType, value) {
        appState.filters = appState.filters || {};
        switch (filterType) {
            case 'search':
                appState.searchTerm = value || '';
                break;
            case 'category':
                appState.currentCategory = value || 'all';
                break;
            case 'only-available':
                appState.filters.onlyAvailable = !!value;
                break;
            case 'price-sort':
                appState.filters.sortByPrice = value || '';
                break;
        }
    }

    // sincroniza controles pares (desktop <-> mobile)
    function syncControls(filterType, value) {
        if (filterType === 'search') {
            document.querySelectorAll('[data-filter="search"]').forEach(el => { if (el !== document.activeElement) el.value = value; });
            return;
        }
        if (filterType === 'category') {
            // select mobile
            const selMobile = document.getElementById('category-select-mobile');
            if (selMobile && selMobile.value !== value) selMobile.value = value;
            // update active class for category buttons
            document.querySelectorAll('.category-btn[data-filter="category"]').forEach(btn => {
                btn.classList.toggle('active', String(btn.dataset.category) === String(value));
            });
            return;
        }
        // checkbox & selects
        document.querySelectorAll(`[data-filter="${filterType}"]`).forEach(el => {
            if (el.type === 'checkbox') el.checked = !!value;
            else if (el.tagName.toLowerCase() === 'select') { if (el.value !== value) el.value = value; }
        });
    }

    // acción que aplica filtros: actualiza appState y pide a DishService que re-renderice
    async function applyFiltersAndRender() {
        try {
            // DishService.filterAndRenderDishes ya usa appState (según tu DishService)
            await DishService.filterAndRenderDishes();
        } catch (err) {
            console.error('Error al aplicar filtros', err);
        }
    }

    // manejadores
    const debouncedApply = debounce(applyFiltersAndRender, 250);

    // attach listeners genéricos
    document.querySelectorAll('[data-filter]').forEach(el => {
        const type = el.dataset.filter;
        if (!type) return;

        if (el.tagName.toLowerCase() === 'input' && el.type === 'text') {
            el.addEventListener('input', (e) => {
                const v = e.target.value;
                updateAppStateFromControl(type, v);
                syncControls(type, v);
                debouncedApply();
            });
        } else if (el.tagName.toLowerCase() === 'input' && el.type === 'checkbox') {
            el.addEventListener('change', (e) => {
                const v = e.target.checked;
                updateAppStateFromControl(type, v);
                syncControls(type, v);
                applyFiltersAndRender();
            });
        } else if (el.tagName.toLowerCase() === 'select') {
            el.addEventListener('change', (e) => {
                const v = e.target.value;
                updateAppStateFromControl(type, v);
                syncControls(type, v);
                applyFiltersAndRender();
            });
        } else if (el.tagName.toLowerCase() === 'button' && el.dataset.category) {
            // category buttons (click)
            el.addEventListener('click', (e) => {
                e.preventDefault();
                const v = el.dataset.category || 'all';
                updateAppStateFromControl('category', v);
                syncControls('category', v);
                applyFiltersAndRender();
            });
        }
    });

    // ensure initial appState defaults if missing
    appState.searchTerm = appState.searchTerm || '';
    appState.currentCategory = appState.currentCategory || 'all';
    appState.filters = appState.filters || {};
    appState.filters.onlyAvailable = appState.filters.onlyAvailable || false;
    appState.filters.sortByPrice = appState.filters.sortByPrice || '';

    // if categories are filled dynamically, sync selects/buttons when categories load
    document.addEventListener('categories:loaded', () => {
        // when your existing code finishes populating categories, sync mobile select
        const desktopActive = document.querySelector('.category-btn.active[data-filter="category"]');
        if (desktopActive) {
            const v = desktopActive.dataset.category || 'all';
            syncControls('category', v);
        }
    });
});

window.hideLoader = hideLoader;