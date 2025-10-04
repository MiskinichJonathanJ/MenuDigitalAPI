import { escapeHtml } from '../utils/helpers.js'; 
import { isValidCategory } from '../utils/validation.js';
export function AllDishesButton(options = {}) {
    const {
        active = true,
        icon = 'th-large',
        text = 'Todos los platos'
    } = options;

    const activeClass = active ? 'active' : '';

    return `
        <li class="mb-2">
            <button class="btn btn-outline-primary w-100 text-start category-btn ${activeClass}" 
                    data-category="all"
                    data-filter="category"
                    aria-label="Mostrar todas las categorías"
                    aria-pressed="${active}">
                <i class="fas fa-${icon} me-2" aria-hidden="true"></i>
                ${escapeHtml(text)}
            </button>
        </li>
    `;
}

export function categoryItemHTML(category) {
    if (!isValidCategory(category)) {
        return '';
    }

    const {
        id,
        name,
        description = '',
        icon = 'utensils',
        active = false
    } = category;

    const activeClass = active ? 'active' : '';
    const descriptionHTML = description ? `
        <small class="d-block text-muted" style="font-size: 0.75rem;">
            ${escapeHtml(description)}
        </small>
    ` : '';

    return `
        <button class="btn btn-outline-primary w-100 text-start category-btn ${activeClass}" 
                data-category="${escapeHtml(String(id))}"
                data-filter="category"
                aria-label="Filtrar por categoría ${escapeHtml(name)}"
                aria-pressed="${active}">
            <i class="fas fa-${icon} me-2" aria-hidden="true"></i>
            <span>${escapeHtml(name)}</span>
            ${descriptionHTML}
        </button>
    `;
}
export function categoryOptionHTML(category) {
    if (!isValidCategory(category)) {
        return '';
    }

    const { id, name, selected = false } = category;
    const selectedAttr = selected ? 'selected' : '';

    return `
        <option value="${escapeHtml(String(id))}" ${selectedAttr}>
            ${escapeHtml(name)}
        </option>
    `;
}

export function renderCategoryList(categories, options = {}) {
    const {
        includeAll = true,
        activeCategory = 'all'
    } = options;

    if (!Array.isArray(categories)) {
        return '';
    }

    let html = '';

    if (includeAll) {
        html += AllDishesButton({ active: activeCategory === 'all' });
    }

    categories.forEach(category => {
        if (!isValidCategory(category)) return;

        const categoryWithActive = {
            ...category,
            active: String(category.id) === String(activeCategory)
        };

        html += `<li class="mb-2">${categoryItemHTML(categoryWithActive)}</li>`;
    });

    return html;
}

export function renderCategorySelect(categories, options = {}) {
    const {
        includeAll = true,
        allText = 'Todas las categorías',
        selectedCategory = 'all'
    } = options;

    if (!Array.isArray(categories)) {
        return '';
    }

    let html = '';

    if (includeAll) {
        const selectedAttr = selectedCategory === 'all' ? 'selected' : '';
        html += `<option value="all" ${selectedAttr}>${escapeHtml(allText)}</option>`;
    }

    categories.forEach(category => {
        if (!isValidCategory(category)) return;

        const categoryWithSelected = {
            ...category,
            selected: String(category.id) === String(selectedCategory)
        };

        html += categoryOptionHTML(categoryWithSelected);
    });

    return html;
}
