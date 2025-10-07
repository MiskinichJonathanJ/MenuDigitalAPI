import { renderCategoryList, renderCategorySelect } from './category.js';

export function updateCategoryUI(categories, activeCategory = 'all') {
    const categoriesList = document.getElementById('categories-list');
    const categorySelect = document.getElementById('category-select-mobile');

    if (categoriesList) {
        categoriesList.innerHTML = renderCategoryList(categories, {
            includeAll: true,
            activeCategory
        });
    }

    if (categorySelect) {
        categorySelect.innerHTML = renderCategorySelect(categories, {
            includeAll: true,
            allText: 'Todas las categorías',
            selectedCategory: activeCategory
        });
    }

    updateBreadcrumb(activeCategory, getCategoryName(categories, activeCategory));

    document.dispatchEvent(new CustomEvent('categories:loaded', {
        detail: { categories, activeCategory }
    }));
}

export function syncCategoryUI(categoryId, category) {
    document.querySelectorAll('.category-btn[data-category]').forEach(btn => {
        const isActive = btn.dataset.category === categoryId;
        btn.classList.toggle('active', isActive);
        btn.setAttribute('aria-pressed', isActive);
    });

    const mobileSelect = document.getElementById('category-select-mobile');
    if (mobileSelect && mobileSelect.value !== categoryId) {
        mobileSelect.value = categoryId;
    }

    const categoryName = category ? category.name : (categoryId === 'all' ? 'Todos los platos' : 'Categoría desconocida');
    updateBreadcrumb(categoryId, categoryName);
}

function updateBreadcrumb(categoryId, categoryName) {
    const breadcrumb = document.getElementById('current-category');
    if (breadcrumb) {
        breadcrumb.textContent = categoryName;
    }
}

function getCategoryName(categories, categoryId) {
    if (categoryId === 'all') return 'Todos los platos';
    const category = categories.find(c => String(c.id) === String(categoryId));
    return category ? category.name : 'Categoría desconocida';
}