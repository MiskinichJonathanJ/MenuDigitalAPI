export function AllDishesButton() {
    return `
        <li class="mb-2">
          <button class="btn btn-outline-primary w-100 text-start category-btn active" data-category="all">
            <i class="fas fa-th-large me-2"></i> Todos los platos
          </button>
        </li>
      `;
}

export function categoryItemHTML(category) {
    return `
        <button class="btn btn-outline-primary w-100 text-start category-btn" data-category="${category.id}">
          <i class="fas fa-utensils me-2"></i>
          ${category.name}
          <small class="d-block text-muted" style="font-size: 0.75rem;">${category.description}</small>
        </button>
      `;
}