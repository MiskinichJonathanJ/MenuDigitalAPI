import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { showError, showSuccess } from '../utils/helpers.js';
import { appState } from '../store.js';
import { modalDishHTML } from '../components/modal.js';
import { dishLoader, errorDishLoader } from '../components/loader.js';
import { dishCardHTML } from '../components/dishCard.js';

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
        const viewButtons = document.querySelectorAll('.view-dish-btn');
        viewButtons.forEach(button => {
            button.addEventListener('click', async (e) => {
                const dishId = e.currentTarget.dataset.dishId;
                await this.showDishDetails(dishId);
            });
        });

        const addButtons = document.querySelectorAll('.add-to-cart-btn');
        addButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const dishId = e.currentTarget.dataset.dishId;
                this.addToCart(dishId);
            });
        });
    },

    async showDishDetails(dishId) {
        try {
            let dish = appState.dishes.find(d => d.id.toString() === dishId.toString());
I
            if (!dish) {
                dish = await this.getDishById(dishId);
            }

            const modalContent = document.getElementById('dish-modal-content');
            const modalTitle = document.getElementById('dishModalLabel');

            if (!modalContent || !modalTitle) {
                return;
            }

            modalTitle.textContent = dish.name;
            modalContent.innerHTML = modalDishHTML(dish);

            const addToCartModalBtn = modalContent.querySelector('.add-to-cart-modal-btn');
            if (addToCartModalBtn) {
                addToCartModalBtn.addEventListener('click', (e) => {
                    const dishId = e.currentTarget.dataset.dishId;
                    this.addToCart(dishId);

                    const modal = bootstrap.Modal.getInstance(document.getElementById('dishModal'));
                    if (modal) modal.hide();
                });
            }

            const modal = new bootstrap.Modal(document.getElementById('dishModal'));
            modal.show();

        } catch (error) {
            if (showError) {
                showError('Error al cargar detalles del plato');
            }
        }
    },

    addToCart(dishId, quantity = 1) {
        try {
            const dish = appState.dishes.find(d => d.id.toString() === dishId.toString());

            if (!dish) {
                throw new Error('Plato no encontrado');
            }

            if (!dish.isActive) {
                if (showError) {
                    showError('Este plato no está disponible');
                }
                return;
            }

            const existingItem = appState.cart.find(item => item.dish.id.toString() === dishId.toString());

            if (existingItem) {
                existingItem.quantity += quantity;
            } else {
                appState.cart.push({
                    id: generateId ? generateId() : Date.now().toString(),
                    dish: dish,
                    quantity: quantity,
                    notes: ''
                });
            }

            if (window.CartService && window.CartService.updateCartUI) {
                window.CartService.updateCartUI();
            }

            if (showSuccess) {
                showSuccess(`${dish.name} agregado al carrito`);
            }

        } catch (error) {
            if (showError) {
                showError('Error al agregar plato al carrito');
            }
        }
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