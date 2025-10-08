import { formatPrice, escapeHtml } from '../utils/helpers.js';
import { isValidDish } from '../utils/validation.js';

const DEFAULT_DISH_IMAGE = 'https://cdni.iconscout.com/illustration/premium/thumb/no-result-found-illustration-svg-download-png-11838290.png';

function renderModalImage(dish) {
    const imageUrl = dish.image || DEFAULT_DISH_IMAGE;
    const altText = escapeHtml(dish.name);

    return `
        <img src="${imageUrl}" 
             class="dish-modal-img img-fluid rounded" 
             alt="${altText}"
             onerror="this.src='${DEFAULT_DISH_IMAGE}'">
    `;
}

function renderModalInfo(dish) {
    const price = formatPrice(dish.price);
    const description = dish.description
        ? escapeHtml(dish.description)
        : '<em class="text-muted">Sin descripción disponible</em>';
    const categoryName = dish.category?.name || 'Sin categoría';
    const isActive = dish.isActive !== false;

    const statusBadge = isActive
        ? '<span class="badge bg-success">Disponible</span>'
        : '<span class="badge bg-danger">No disponible</span>';

    return `
        <p class="dish-modal-price fs-4 fw-bold text-primary mb-3">
            ${price}
        </p>
        
        <div class="mb-3">
            <h6 class="text-muted mb-2">Descripción</h6>
            <p class="mb-0">${description}</p>
        </div>

        <div class="mb-2">
            <strong>Categoría:</strong> 
            <span class="text-muted">${escapeHtml(categoryName)}</span>
        </div>
        
        <div class="mb-3">
            <strong>Estado:</strong> 
            ${statusBadge}
        </div>
    `;
}

function renderModalForm(dish) {
    const dishId = escapeHtml(String(dish.id));

    return `
        <div class="mb-3">
            <label for="dish-quantity" class="form-label fw-bold">
                Cantidad
            </label>
            <input id="dish-quantity" 
                   class="form-control dish-quantity-input" 
                   type="number" 
                   min="1" 
                   max="99"
                   value="1"
                   aria-label="Cantidad de porciones"
                   required />
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
        return `
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-exclamation-circle me-2" aria-hidden="true"></i>
                <strong>Error</strong>
                <p class="mb-0 mt-2">No se pudo cargar la información del plato.</p>
            </div>
        `;
    }

    const isActive = dish.isActive !== false;

    return `
        <div class="row">
            <div class="col-md-6 mb-3 mb-md-0">
                ${renderModalImage(dish)}
            </div>
            
            <div class="col-md-6">
                ${renderModalInfo(dish)}
                
                ${isActive
            ? renderModalForm(dish)
            : renderUnavailableMessage()
        }
            </div>
        </div>
    `;
}