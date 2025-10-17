import { formatPrice, escapeHtml } from '../utils/helpers.js';
import { isValidDish } from '../utils/validation.js';
import { DEFAULT_DISH_IMAGE } from "../config/constants.js";

function renderModalForm(dish) {
    const dishId = escapeHtml(String(dish.id));

    return `
        <div class="mb-3">
            <label for="dish-quantity" class="form-label fw-bold">
                Cantidad
            </label>
            <div class="input-group">
                <button type="button" class="btn btn-outline-secondary" data-action="decrease">-</button>
                <input id="dish-quantity"
                       class="form-control dish-quantity-input text-center" 
                       type="number" 
                       min="1" 
                       max="99"
                       value="1"
                       aria-label="Cantidad de porciones"
                       required />
                <button type="button" class="btn btn-outline-secondary" data-action="increase">+</button>
            </div>
            <div class="form-text">Mínimo: 1, Máximo: 99</div>
        </div>

        <div class="mb-3">
            <label for="dish-notes" class="form-label fw-bold">
                Notas adicionales 
                <span class="text-muted fw-normal">(opcional)</span>
            </label>
            <textarea id="dish-notes" 
                      class="form-control dish-notes-input" 
                      rows="3" 
                      maxlength="200"
                      placeholder="Ej: Sin sal, 1/2 cocción, sin cebolla..."
                      aria-label="Notas adicionales para el pedido"></textarea>
            <div class="form-text">Máximo 200 caracteres</div>
        </div>

        <div class="d-grid gap-2">
            <button class="btn btn-primary btn-lg add-to-cart-modal-btn" 
                    data-dish-id="${dishId}"
                    aria-label="Agregar ${escapeHtml(dish.name)} al carrito">
                <i class="fas fa-plus me-2" aria-hidden="true"></i>
                Agregar al carrito
            </button>
        </div>
    `;
}


function renderUnavailableMessage() {
    return `
        <div class="alert alert-warning" role="alert">
            <i class="fas fa-exclamation-triangle me-2" aria-hidden="true"></i>
            <strong>Plato no disponible</strong>
            <p class="mb-0 mt-2">
                Este plato no está disponible en este momento. 
                Por favor, selecciona otro plato del menú.
            </p>
        </div>
    `;
}


export function modalDishHTML(dish) {
    if (!isValidDish(dish)) {
        return renderUnavailableMessage();
    }

    const isActive = dish.isActive !== false;
    const price = formatPrice(dish.price);
    const image = dish.image || DEFAULT_DISH_IMAGE;
    const description = dish.description
        ? escapeHtml(dish.description)
        : '<em class="text-muted">Sin descripción disponible</em>';
    const categoryName = dish.category?.name || 'Sin categoría';

    return `
        <div class="dish-modal-content">
            <div class="dish-modal-left">
                <img src="${image}" 
                     class="dish-modal-img"
                     alt="${escapeHtml(dish.name)}"
                     loading="lazy"
                     onerror="this.src='${DEFAULT_DISH_IMAGE}'">
            </div>

            <div class="dish-modal-right">
                <h3>${escapeHtml(dish.name)}</h3>
                <div class="category"><strong>Categoría:</strong> ${escapeHtml(categoryName)}</div>
                <div class="price">${price}</div>
                <div class="description">${description}</div>

                ${isActive ? renderModalForm(dish) : `
                    <div class="alert bg-dark">
                        <i class="fas fa-ban me-2"></i>
                        Este plato no está disponible actualmente.
                    </div>
                `}
            </div>
        </div>
    `;
}

