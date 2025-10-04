import { toastHTML } from "../components/toast.js";
function getOrCreateToastContainer() {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        document.body.appendChild(container);
    }
    return container;
}
function showToast(message, type) {
    const container = getOrCreateToastContainer();
    const toastHtmlContent = toastHTML(message, type);

    container.insertAdjacentHTML('beforeend', toastHtmlContent);

    const toastElement = container.lastElementChild;
    const bsToast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: type === 'error' ? 5000 : 3000 
    });

    bsToast.show();

    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
}

export function showMessage(message, type = 'info') {
    if (!message || typeof message !== 'string') {
        console.warn('showMessage: mensaje inválido', message);
        return;
    }

    const cleanMessage = message.trim();
    if (cleanMessage === '') {
        console.warn('showMessage: mensaje vacío');
        return;
    }

    const validTypes = ['success', 'error', 'warning', 'info'];
    const normalizedType = validTypes.includes(type) ? type : 'info';

    showToast(cleanMessage, normalizedType);
}
export function showError(message) {
    showMessage(message, 'error')
}

export function showSuccess(message) {
    showMessage(message, 'success')
}

export function formatPrice(price) {
    return new Intl.NumberFormat('es-AR', {
        style: 'currency',
        currency: 'ARS',
        minimumFractionDigits: 0
    }).format(price);
}

export function generateId() {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
}

export function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}
export function escapeHtml(str) {
    return String(str)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
}
export function truncate(text, maxLength) {
    if (typeof text !== 'string') return '';
    return text.length > maxLength
        ? text.slice(0, maxLength) + '…'
        : text;
}
