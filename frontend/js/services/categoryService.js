import { appStore } from '../appStore.js';
import { showError } from '../utils/helpers.js';
import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { updateCategoryUI, syncCategoryUI } from '../components/categoryUI.js';

const CategoryService = {
    async getAllCategories() {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.CATEGORIES);
            const categories = await apiRequest(url);

            if (!Array.isArray(categories)) {
                throw new Error('Respuesta inválida del servidor');
            }

            return categories;
        } catch (error) {
            throw new Error(`Error al cargar categorías: ${error.message}`);
        }
    },

    getCategoryById(categoryId) {
        const categories = appStore.getState('categories');
        return categories.find(cat => String(cat.id) === String(categoryId)) || null;
    },

    setCurrentCategory(categoryId) {
        const normalizedId = categoryId === 'all' ? 'all' : String(categoryId);
        appStore.setState('currentCategory', normalizedId);
    },

    setupEventListeners() {
        document.addEventListener('click', (e) => {
            const categoryBtn = e.target.closest('.category-btn[data-category]');
            if (!categoryBtn) return;

            e.preventDefault();
            this.setCurrentCategory(categoryBtn.dataset.category);
        });

        const mobileSelect = document.getElementById('category-select-mobile');
        if (mobileSelect) {
            mobileSelect.addEventListener('change', (e) => {
                this.setCurrentCategory(e.target.value);
            });
        }

        appStore.subscribe('currentCategory', (newCategory) => {
            syncCategoryUI(newCategory, this.getCategoryById(newCategory));
        });
    },

    async init() {
        try {
            const categories = await this.getAllCategories();
            appStore.setState('categories', categories);

            const currentCategory = appStore.getState('currentCategory');
            updateCategoryUI(categories, currentCategory);

            this.setupEventListeners();

            return categories;
        } catch (error) {
            showError('Error al cargar categorías. Verifique la conexión.');
            updateCategoryUI([], 'all');
            this.setupEventListeners();
            throw error;
        }
    }
};

export { CategoryService };