import { showError, showSuccess, debounce } from './utils/helpers.js';
import { appState } from './store.js';
import { renderLoader } from './components/loader.js';
import { CategoryService } from './services/categoryService.js';
import { DishService } from './services/dishService.js';

async function initApp() {
    try {
        const loadingDishes = document.getElementById('loading-dishes');
        if (loadingDishes) {
            loadingDishes.innerHTML = renderLoader();
        }

        if (CategoryService) {
            await CategoryService.init();
        }
        if (DishService) {
            await DishService.init();
        }

        showSuccess('Aplicación cargada correctamente!');
    } catch (error) {
        const errorMessage = error.message || 'Error desconocido';
        showError(`Error al cargar la aplicación: ${errorMessage}`);
    }
}

function setupEventListeners() {
    const searchInput = document.getElementById('search-input');
    const searchBtn = document.getElementById('search-btn');

    if (searchInput && searchBtn) {
        searchInput.addEventListener('input', debounce((e) => {
            appState.searchTerm = e.target.value;
            if (DishService?.filterAndRenderDishes) {
                DishService.filterAndRenderDishes();
            }
        }, 300));

        searchBtn.addEventListener('click', () => {
            if (DishService?.filterAndRenderDishes) {
                DishService.filterAndRenderDishes();
            }
        });
    }

    const onlyAvailableCheckbox = document.getElementById('only-available');
    if (onlyAvailableCheckbox) {
        onlyAvailableCheckbox.addEventListener('change', (e) => {
            appState.filters.onlyAvailable = e.target.checked;
            if (DishService?.filterAndRenderDishes) {
                DishService.filterAndRenderDishes();
            }
        });
    }

    const priceSortSelect = document.getElementById('price-sort');
    if (priceSortSelect) {
        priceSortSelect.addEventListener('change', (e) => {
            appState.filters.sortByPrice = e.target.value;
            if (DishService?.filterAndRenderDishes) {
                DishService.filterAndRenderDishes();
            }
        });
    }
}

export { initApp, setupEventListeners };
