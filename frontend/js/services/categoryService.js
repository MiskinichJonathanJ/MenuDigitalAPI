import { appState } from '../store.js';
import { DishService } from './dishService.js';
import { showError } from '../utils/helpers.js';
import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { AllDishesButton, categoryItemHTML } from "../components/category.js";

const CategoryService = {
    async getAllCategories() {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.CATEGORIES);
            console.log('Fetching categories from URL:', url);
            const response = await apiRequest(url);
            return response;
        } catch (error) {
            throw new Error(`Error al cargar categorías: ${error.message}`);
        }
    },

    renderCategories(categories) {
        const categoriesList = document.getElementById('categories-list');
        const categorySelectMobile = document.getElementById('category-select-mobile');

        if (!categoriesList) {
            return;
        }

        const allButton = categoriesList.querySelector('[data-category="all"]')?.parentElement;
        categoriesList.innerHTML = '';

        if (allButton) {
            categoriesList.appendChild(allButton);
        } else {
            categoriesList.innerHTML = AllDishesButton();
        }

        categories.forEach(category => {
            const categoryItem = document.createElement('li');
            categoryItem.className = 'mb-2';
            categoryItem.innerHTML = categoryItemHTML(category);
            categoriesList.appendChild(categoryItem);
        });

        if (categorySelectMobile) {
            categorySelectMobile.innerHTML = '<option value="all">Todas las categorías</option>';
            categories.forEach(category => {
                const option = document.createElement('option');
                option.value = category.id;
                option.textContent = category.name; 
                categorySelectMobile.appendChild(option);
            });
        }

        this.setupCategoryEventListeners();
        document.dispatchEvent(new Event('categories:loaded'));
    }, 

    setupCategoryEventListeners() {
        const categoryButtons = document.querySelectorAll('.category-btn');
        const categorySelectMobile = document.getElementById('category-select-mobile');

        categoryButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                categoryButtons.forEach(btn => btn.classList.remove('active'));

                e.currentTarget.classList.add('active');

                const categoryId = e.currentTarget.dataset.category;
                appState.currentCategory = categoryId;

                this.updateBreadcrumb(categoryId, e.currentTarget.textContent.trim());

                if (DishService && DishService.filterAndRenderDishes) {
                    DishService.filterAndRenderDishes();
                }
            });
        });

        if (categorySelectMobile) {
            categorySelectMobile.addEventListener('change', (e) => {
                const categoryId = e.target.value;

                appState.currentCategory = categoryId;

                categoryButtons.forEach(btn => {
                    btn.classList.remove('active');
                    if (btn.dataset.category === categoryId) {
                        btn.classList.add('active');
                    }
                });

                const selectedOption = e.target.options[e.target.selectedIndex];
                this.updateBreadcrumb(categoryId, selectedOption.textContent);

                if (DishService && DishService.filterAndRenderDishes) {
                    DishService.filterAndRenderDishes();
                }
            });
        }
    },

    updateBreadcrumb(categoryId, categoryName) {
        const breadcrumb = document.getElementById('current-category');
        if (breadcrumb) {
            breadcrumb.textContent = categoryName;
        }
    },
    getCategoryById(categoryId) {
        return appState.categories.find(cat => cat.id.toString() === categoryId.toString());
    },

    async init() {
        try {
            const categories = await this.getAllCategories();

            appState.categories = categories;

            this.renderCategories(categories);

            return categories;

        } catch (error) {
            if (showError) {
                showError('Error al cargar categorías. Verifique la conexión.');
            }
            this.renderCategories([]);
            throw error;
        }
    }
};

export { CategoryService };