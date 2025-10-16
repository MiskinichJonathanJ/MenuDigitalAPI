export function renderMobileFilters() {
    return `
        <div class="mb-3">
            <select id="category-select-mobile" class="form-select form-select-sm">
                <option value="all">Todas las categorías</option>
            </select>
        </div>
        <div class="mb-3">
            <input type="text" id="search-input-mobile" class="form-control form-control-sm"
                placeholder="Buscar platos...">
        </div>
        <div class="mb-2">
            <select id="price-sort-mobile" class="form-select form-select-sm">
                <option value="">Sin Orden</option>
                <option value="asc">Menor a mayor</option>
                <option value="desc">Mayor a menor</option>
            </select>
        </div>
    `;
}