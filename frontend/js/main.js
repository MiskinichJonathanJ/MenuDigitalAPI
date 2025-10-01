import { initApp, setupEventListeners } from './app.js';

function hideLoader() {
    const loader = document.getElementById('loader');
    if (loader) {
        loader.classList.add('fade-out');
        setTimeout(() => {
            loader.style.display = 'none';
        }, 300);
    }
}
document.addEventListener('DOMContentLoaded', () => {
    setupEventListeners();
    initApp();
});

window.hideLoader = hideLoader;