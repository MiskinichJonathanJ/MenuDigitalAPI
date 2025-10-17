import { SELECTORS } from '../config/constants.js';
export function setupBrandingNavigation() {
    const showMenuBtn = document.getElementById(SELECTORS.SHOW_MENU_BTN);
    const showOrdersBtn = document.getElementById(SELECTORS.SHOW_ORDERS_BTN);
    const branding = document.getElementById(SELECTORS.BRANDING_SCREEN);
    const menu = document.getElementById(SELECTORS.MENU_SECTION);
    const orders = document.getElementById(SELECTORS.ORDERS_SECTION);

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
