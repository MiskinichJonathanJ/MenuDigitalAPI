import { initApp, setupEventListeners, setupErrorHandling } from './app.js';

function hideLoader() {
    const loader = document.getElementById('loader');
    if (!loader) return;

    loader.classList.add('fade-out');
    setTimeout(() => {
        loader.style.display = 'none';
        loader.remove();
    }, 300);
}

async function main() {
    try {
        setupErrorHandling();
        setupEventListeners();
        await initApp();
        hideLoader();
    } catch (error) {
        const container = document.getElementById('dishes-container');
        if (container) {
            container.innerHTML = `
                <div class="col-12 text-center py-5">
                    <i class="fas fa-exclamation-triangle text-danger mb-3" 
                       style="font-size: 3rem;"></i>
                    <h4 class="text-muted">Error al iniciar la aplicación</h4>
                    <p class="text-muted">${error.message}</p>
                    <button class="btn btn-primary mt-3" onclick="location.reload()">
                        <i class="fas fa-redo me-2"></i>
                        Recargar página
                    </button>
                </div>
            `;
        }
        hideLoader();
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', main);
} else {
    main();
}

window.hideLoader = hideLoader;