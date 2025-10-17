import { formatPrice, escapeHtml, truncate } from "../utils/helpers.js";
import { isValidDish } from "../utils/validation.js";

const AVAILABILITY_CONFIG = {
    available: {
        class: 'dish-available',
        text: 'Disponible',
        badge: 'bg-success'
    },
    unavailable: {
        class: 'dish-unavailable',
        text: 'No disponible',
        badge: 'bg-danger'
    }
};

import { DEFAULT_DISH_IMAGE } from "../config/constants.js";
export function renderActionButtons(dish) {
    const viewButton = `
        <button class="btn btn-sm btn-outline-primary flex-fill view-dish-btn" 
                data-dish-id="${dish.id}"
                aria-label="Ver detalles de ${escapeHtml(dish.name)}">
            <i class="fas fa-eye me-1" aria-hidden="true"></i>
            Ver detalles
        </button>
    `;

    const addButton = dish.isActive ? `
        <button class="btn btn-sm btn-primary add-to-cart-btn" 
                data-dish-id="${dish.id}"
                aria-label="Agregar ${escapeHtml(dish.name)} al carrito">
            <i class="fas fa-plus" aria-hidden="true"></i>
        </button>
    ` : `
        <button class="btn btn-sm btn-secondary" 
                disabled
                aria-label="Plato no disponible">
            <i class="fas fa-ban" aria-hidden="true"></i>
        </button>
    `;

    return `
        <div class="mt-3 d-flex gap-2">
            ${viewButton}
            ${addButton}
        </div>
    `;
}
export function dishCardHTML(dish, options = {}) {

    if (!isValidDish(dish)) {
        return '';
    }

    const {
        maxDescriptionLength = 100,
        columnClass = 'col-lg-4 col-md-6 col-sm-12'
    } = options;

    const dishId = escapeHtml(String(dish.id));
    const dishName = escapeHtml(dish.name);
    const dishDescription = dish.description
        ? escapeHtml(truncate(dish.description, maxDescriptionLength))
        : '<em class="text-muted">Sin descripción</em>';
    const dishImage = dish.image || DEFAULT_DISH_IMAGE;
    const dishPrice = formatPrice(dish.price);
    const isActive = dish.isActive !== false;
    const availabilityClass = isActive ? 'dish-available' : 'dish-unavailable';

    return `
        <div class="${columnClass} mb-4">
            <article class="card bg-secondary text-light h-100 shadow-sm border-0 ${availabilityClass}"
                     data-dish-id="${dishId}"
                     itemscope itemtype="https://schema.org/MenuItem">
                
                <img src="${dishImage}"
                     class="card-img-top rounded-top dish-card-img"
                     alt="${dishName}"
                     itemprop="image"
                     loading="lazy"
                     onerror="this.src='${DEFAULT_DISH_IMAGE}'">

                <div class="card-body d-flex flex-column">
                    <h5 class="card-title fw-semibold mb-2" itemprop="name">
                        ${dishName}
                    </h5>

                    <p class="card-text text-light flex-grow-1 small mb-3" itemprop="description">
                        ${dishDescription}
                    </p>

                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <strong class="fs-5 dish-card-price" itemprop="offers" itemscope itemtype="https://schema.org/Offer">
                            <meta itemprop="priceCurrency" content="ARS">
                            <span itemprop="price">${dishPrice}</span>
                        </strong>
                        ${isActive
            ? '<span class="badge bg-success">Disponible</span>'
            : '<span class="badge bg-danger">No disponible</span>'
        }
                    </div>

                    ${renderActionButtons(dish)}
                </div>
            </article>
        </div>
    `;
}


export function renderDishCards(dishes, options = {}) {
    if (!Array.isArray(dishes)) {
        return '';
    }

    if (dishes.length === 0) {
        return '';
    }

    return dishes
        .map(dish => dishCardHTML(dish, options))
        .filter(html => html !== '') 
        .join('');
}