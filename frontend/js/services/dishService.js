import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { showError } from '../utils/helpers.js';
import { appStore } from '../appStore.js';
import { updateDishesUI, showDishModal } from '../components/dishUI.js';
import { CartService } from './cartService.js';

const DishService = {
    async getAllDishes(filters = {}) {
        try {
            const params = new URLSearchParams();

            if (filters.name?.trim()) {
                params.append('name', filters.name.trim());
            }

            if (filters.category && filters.category !== 'all') {
                params.append('category', filters.category);
            }

            if (filters.onlyActive !== undefined) {
                params.append('onlyActive', filters.onlyActive);
            }

            if (filters.sortByPrice) {
                params.append('sortByPrice', filters.sortByPrice);
            }

            const queryString = params.toString();
            const url = buildApiUrl(
                API_CONFIG.ENDPOINTS.DISHES,
                queryString ? `?${queryString}` : ''
            );

            const dishes = await apiRequest(url);

            if (!Array.isArray(dishes)) {
                throw new Error('Respuesta inválida del servidor');
            }

            return dishes;
        } catch (error) {
            throw new Error(`Error al cargar platos: ${error.message}`);
        }
    },

    async getDishById(dishId) {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.DISHES, `/${dishId}`);
            const dish = await apiRequest(url);

            if (!dish || typeof dish !== 'object') {
                throw new Error('Plato no encontrado');
            }

            return dish;
        } catch (error) {
            showError(`Error al cargar plato: ${error.message}`);
            throw error;
        }
    },

    async findDish(dishId) {
        const dishes = appStore.getState('dishes');
        let dish = dishes.find(d => String(d.id) === String(dishId));

        if (!dish) {
            dish = await this.getDishById(dishId);
        }

        return dish;
    },

    getCurrentFilters() {
        return {
            name: appStore.getState('searchTerm') || '',
            category: appStore.getState('currentCategory') || 'all',
            onlyActive: appStore.getState('filters')?.onlyAvailable || false,
            sortByPrice: appStore.getState('filters')?.sortByPrice || ''
        };
    },

    async filterAndRenderDishes() {
        try {
            const filters = this.getCurrentFilters();
            const dishes = await this.getAllDishes(filters);

            appStore.setState('dishes', dishes);
            updateDishesUI(dishes);
        } catch (error) {
            updateDishesUI([], true);
        }
    },

    async showDetails(dishId) {
        try {
            const dish = await this.findDish(dishId);
            showDishModal(dish, (payload) => {
                CartService.addToCart(payload);
            });
        } catch (error) {
            showError('Error al cargar detalles del plato');
        }
    },

    setupEventListeners() {
        document.addEventListener('click', async (e) => {
            const viewBtn = e.target.closest('.view-dish-btn');
            if (viewBtn) {
                const dishId = viewBtn.dataset.dishId;
                if (dishId) await this.showDetails(dishId);
                return;
            }

            const addBtn = e.target.closest('.add-to-cart-btn');
            if (addBtn && !addBtn.closest('.modal')) {
                const dishId = addBtn.dataset.dishId;
                if (dishId) CartService.addToCart(dishId);
                return;
            }
        });
    },

    setupStoreListeners() {
        appStore.subscribe('searchTerm', () => this.filterAndRenderDishes());
        appStore.subscribe('currentCategory', () => this.filterAndRenderDishes());
        appStore.subscribe('filters', () => this.filterAndRenderDishes());
    },

    async init() {
        try {
            this.setupStoreListeners();
            this.setupEventListeners();
            await this.filterAndRenderDishes();
        } catch (error) {
            throw error;
        }
    }
};


export { DishService };