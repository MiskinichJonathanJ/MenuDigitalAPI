import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { showError } from '../utils/helpers.js';
import { appState } from '../store.js';
import { dishLoader, errorDishLoader } from '../components/loader.js';
import { dishCardHTML } from '../components/dishCard.js';
import { DishModal } from "./dishModal.js";
import { CartService } from "./cartService.js";

const DishService = {
    async getAllDishes(filters = {}) {
        try {
            const params = new URLSearchParams();

            if (filters.name) params.append('name', filters.name);

            if (filters.category && filters.category !== 'all') {
                params.append('category', filters.category);
            }

            if (filters.onlyActive !== undefined) params.append('onlyActive', filters.onlyActive);
            if (filters.sortByPrice) params.append('sortByPrice', filters.sortByPrice);

            const url = buildApiUrl(API_CONFIG.ENDPOINTS.DISHES, params.toString() ? `?${params.toString()}` : '');

            const response = await apiRequest(url);
            return response;
        } catch (error) {
            showError(`Error al cargar platos: ${error.message}`);
            return [];
        }
    },

    async getDishById(dishId) {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.DISHES, `/${dishId}`);
            const response = await apiRequest(url);
            return response;
        } catch (error) {
            showError(error.message);
            return null;
        }
    },

    renderDishes(dishes) {
        const dishesContainer = document.getElementById('dishes-container');
        const loadingDishes = document.getElementById('loading-dishes');
        const noDishes = document.getElementById('no-dishes');

        if (!dishesContainer) {
            return;
        }

        if (loadingDishes) loadingDishes.style.display = 'none';
        if (noDishes) noDishes.classList.add('d-none');

        if (!dishes || dishes.length === 0) {
            dishesContainer.innerHTML = '';
            if (noDishes) noDishes.classList.remove('d-none');
            return;
        }

        const dishesHTML = dishes.map(dish => dishCardHTML(dish)).join('');
        dishesContainer.innerHTML = dishesHTML;

        this.setupDishEventListeners();
    },

    setupDishEventListeners() {
        document.querySelectorAll('.view-dish-btn').forEach(button => {
            button.addEventListener('click', (e) => {
                const dishId = e.currentTarget.dataset.dishId;
                console.log(dishId);
                if (DishModal) DishModal.show(dishId);
            });
        });
        document.querySelectorAll('.add-to-cart-btn').forEach(button => {
            button.addEventListener('click', (e) => {
                const dishId = e.currentTarget.dataset.dishId;
                if (CartService) CartService.addToCart(dishId);
            });
        });
    },
    async filterAndRenderDishes() {
        try {
            const loadingDishes = document.getElementById('loading-dishes');

            if (loadingDishes) {
                loadingDishes.style.display = 'block';
                loadingDishes.innerHTML = dishLoader();
            }

            const filters = {
                name: appState.searchTerm,
                category: appState.currentCategory,
                onlyActive: appState.filters.onlyAvailable,
                sortByPrice: appState.filters.sortByPrice
            };

            const dishes = await this.getAllDishes(filters);
            appState.dishes = dishes;

            this.renderDishes(dishes);

        } catch (error) {
            if (showError) {
                showError('Error al cargar platos');
            }

            const dishesContainer = document.getElementById('dishes-container');
            if (dishesContainer) {
                dishesContainer.innerHTML = errorDishLoader();
            }
        }
    },

    async init() {
        try {
            await this.filterAndRenderDishes();
        } catch (error) {
            throw error;
        }
    }
};

export { DishService };