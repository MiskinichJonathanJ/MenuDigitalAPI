const LOADER_CONFIG = {
    default: {
        icon: 'spinner-border',
        title: 'Cargando...',
        message: 'Por favor espere',
        variant: 'primary'
    },
    dishes: {
        icon: 'spinner-border',
        title: 'Cargando platos...',
        message: 'Obteniendo el menú',
        variant: 'primary'
    },
    categories: {
        icon: 'spinner-border',
        title: 'Cargando categorías...',
        message: 'Obteniendo categorías',
        variant: 'primary'
    },
    api: {
        icon: 'spinner-border',
        title: 'Conectando con el servidor...',
        message: 'Estableciendo conexión',
        variant: 'primary'
    }
};   
export function showErrorState(container, { title, message, retryAction = null }) {
    if (!container) return;

    const retryButton = retryAction
        ? `<button class="btn btn-outline-primary mt-3" onclick="${retryAction}()">Reintentar</button>`
        : '';

    const errorHTML = `
        <div class="col-12 text-center py-5">
            <div class="text-danger">
                <i class="bi bi-exclamation-triangle fs-1"></i>
            </div>
            <h4 class="mt-3 text-danger">${title || 'Error'}</h4>
            <p class="text-muted">${message || 'Ha ocurrido un error inesperado'}</p>
            ${retryButton}
        </div>
    `;

    container.innerHTML = errorHTML;
}