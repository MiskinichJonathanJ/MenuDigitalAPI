import { formatPrice } from "../utils/helpers.js";

export function dishCardHTML(dish) {
    const availabilityClass = dish.isActive ? 'dish-available' : 'dish-unavailable';
    const availabilityText = dish.isActive ? 'Disponible' : 'No disponible';

    return `
          <div class="col-lg-4 col-md-6 col-sm-12 mb-4">
            <div class="card dish-card h-100" data-dish-id="${dish.id}">
              <div class="position-relative">
                <img src="${dish.image}" class="card-img-top dish-card-img" alt="${dish.name}" loading="lazy">
                <span class="category-badge">${dish.category?.name || 'Sin categoría'}</span>
              </div>
          
              <div class="card-body dish-card-body">
                <h5 class="dish-card-title">${dish.name}</h5>
                <p class="dish-card-description">${dish.description}</p>
            
                <div class="dish-card-footer">
                  <div class="d-flex justify-content-between align-items-center">
                    <span class="dish-card-price">${formatPrice(dish.price)}</span>
                    <span class="dish-availability ${availabilityClass}">${availabilityText}</span>
                  </div>
              
                  <div class="mt-3 d-flex gap-2">
                    <button class="btn btn-sm btn-outline-primary flex-fill view-dish-btn" data-dish-id="${dish.id}">
                      <i class="fas fa-eye me-1"></i>
                      Ver detalles
                    </button>
                    ${dish.isActive ? `
                      <button class="btn btn-sm btn-primary add-to-cart-btn" data-dish-id="${dish.id}">
                        <i class="fas fa-plus"></i>
                      </button>
                    ` : `
                      <button class="btn btn-sm btn-secondary" disabled>
                        <i class="fas fa-ban"></i>
                      </button>
                    `}
                  </div>
                </div>
              </div>
            </div>
          </div>
    `;
}