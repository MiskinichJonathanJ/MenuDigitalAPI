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

export function renderLoader(options = {}) {
    const {
        type = 'default',
        title = null,
        message = null,
        variant = 'primary',
        size = 'md'
    } = options;

    const config = LOADER_CONFIG[type] || LOADER_CONFIG.default;

    const finalTitle = title || config.title;
    const finalMessage = message || config.message;
    const finalVariant = variant || config.variant;

    const sizeClass = {
        sm: 'spinner-border-sm',
        md: '',
        lg: 'spinner-border-lg'
    }[size] || '';

    return `
        <div class="col-12 text-center py-5" role="status" aria-live="polite">
            <div class="${config.icon} text-${finalVariant} ${sizeClass}" role="status">
                <span class="visually-hidden">${finalTitle}</span>
            </div>
            <h4 class="mt-3 text-muted">${finalTitle}</h4>
            <p class="text-muted">${finalMessage}</p>
        </div>
    `;
}   

export function dishLoader() {
    return renderLoader({ type: 'dishes' });
}

export function errorDishLoader() {
    return renderErrorState({
        title: 'Error al cargar platos',
        message: 'Verifique la conexión con la API',
        retryAction: 'window.DishService.filterAndRenderDishes'
    });
}