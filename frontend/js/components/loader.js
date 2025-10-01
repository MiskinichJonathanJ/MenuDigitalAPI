export function renderLoader(message = "Conectando con la API...") {
    return `
    <div class="col-12 text-center py-5">
      <h4 class="text-muted">${message}</h4>
      <p class="text-muted mb-4">Cargando datos del restaurante</p>
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Conectando...</span>
      </div>
    </div>
  `;
}

export function dishLoader() {
    return `
    <div class="col-12 text-center py-5">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando platos...</span>
        </div>
        <p class="mt-2 text-muted">Filtrando platos...</p>
    </div>`;
}

export function errorDishLoader() {
    return `<div class="col-12 text-center py-5">
                <i class="fas fa-exclamation-triangle text-warning mb-3" style="font-size: 3rem;"></i>
                <h4 class="text-muted">Error al cargar platos</h4>
                <p class="text-muted">Verifique la conexión con la API</p>
                <button class="btn btn-primary" onclick="window.DishService.filterAndRenderDishes()">
                  <i class="fas fa-redo me-2"></i>Reintentar
                </button>
            </div> `;
}