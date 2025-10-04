import { escapeHtml } from '../utils/helpers.js';

export function toastHTML(message, type = 'success') {
    const config = {
        success: { bg: 'bg-success', icon: 'fa-check-circle', ariaLive: 'polite' },
        error: { bg: 'bg-danger', icon: 'fa-exclamation-circle', ariaLive: 'assertive' },
        warning: { bg: 'bg-warning', icon: 'fa-exclamation-triangle', ariaLive: 'polite' },
        info: { bg: 'bg-info', icon: 'fa-info-circle', ariaLive: 'polite' }
    };

    const { bg, icon, ariaLive } = config[type] || config.success;

    return `
        <div class="toast align-items-center text-white ${bg} border-0" 
             role="alert" 
             aria-live="${ariaLive}" 
             aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas ${icon} me-2" aria-hidden="true"></i>
                    ${escapeHtml(message)}
                </div>
                <button type="button" 
                        class="btn-close btn-close-white me-2 m-auto" 
                        data-bs-dismiss="toast" 
                        aria-label="Cerrar notificación"></button>
            </div>
        </div>
    `;
}