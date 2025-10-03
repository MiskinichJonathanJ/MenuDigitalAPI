// modal.js
import { formatPrice } from '../utils/helpers.js';
export function modalDishHTML(dish) {
    return `
    <div class="row">
      <div class="col-md-6">
        <img src="${dish.image || 'https://i.etsystatic.com/24098368/r/il/35354b/2474697613/il_340x270.2474697613_86xp.jpg'}" 
             class="dish-modal-img" alt="${dish.name}">
      </div>
      <div class="col-md-6">
        <p class="dish-modal-price">${formatPrice ? formatPrice(dish.price) : `$${dish.price}`}</p>
        <p class="mb-3">${dish.description || ''}</p>

        <div class="mb-2"><strong>Categoría:</strong> ${dish.category?.name || 'Sin categoría'}</div>
        <div class="mb-3">
          <strong>Estado:</strong>
          <span class="badge ${dish.isActive ? 'bg-success' : 'bg-danger'}">
            ${dish.isActive ? 'Disponible' : 'No disponible'}
          </span>
        </div>

        ${dish.isActive ? `
          <div class="mb-3">
            <label for="dish-quantity" class="form-label">Cantidad</label>
            <input id="dish-quantity" class="form-control dish-quantity-input" type="number" min="1" value="1" />
          </div>

          <div class="mb-3">
            <label for="dish-notes" class="form-label">Notas (opcional)</label>
            <textarea id="dish-notes" class="form-control dish-notes-input" rows="2" placeholder="Ej: 1/2 cocción, sin sal"></textarea>
          </div>

          <div class="d-grid gap-2">
            <button class="btn btn-primary btn-lg add-to-cart-modal-btn" data-dish-id="${dish.id}">
              <i class="fas fa-plus me-2"></i> Agregar al carrito
            </button>
          </div>
        ` : ''}

      </div>
    </div>
  `
}
