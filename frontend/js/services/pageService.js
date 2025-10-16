export function setupBrandingNavigation() {
    const showMenuBtn = document.getElementById('show-menu-btn');
    const showOrdersBtn = document.getElementById('show-orders-btn');
    const branding = document.getElementById('branding-screen');
    const menu = document.getElementById('menu-section');
    const orders = document.getElementById('orders-section');

    if (showMenuBtn) {
        showMenuBtn.addEventListener('click', () => {
            branding.classList.add('d-none');
            orders.classList.add('d-none');
            menu.classList.remove('d-none');
        });
    }

    if (showOrdersBtn) {
        showOrdersBtn.addEventListener('click', () => {
            branding.classList.add('d-none');
            menu.classList.add('d-none');
            orders.classList.remove('d-none');
        });
    }
    
}
