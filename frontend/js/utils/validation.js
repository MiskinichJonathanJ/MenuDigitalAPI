import { MESSAGES } from '../config/constants.js';
import { showMessage } from './helpers.js';
export function isValidId(id) {
    return id !== null && id !== undefined && String(id).trim() !== '';
}

export function isValidQuantity(qty) {
    const num = Number(qty);
    return Number.isInteger(num) && num >= 1;
}

export function isValidPrice(price) {
    const num = Number(price);
    return !isNaN(num) && num >= 0;
}

export function isValidDish(dish) {
    if (!dish || typeof dish !== 'object') return false;
    return isValidId(dish.id) &&
        typeof dish.name === 'string' &&
        dish.name.trim() !== '' &&
        isValidPrice(dish.price);
}

export function isValidCartItem(item) {
    if (!item || typeof item !== 'object') return false;
    return isValidId(item.id) &&
        isValidDish(item.dish) &&
        isValidQuantity(item.quantity);
}
export  function isValidCategory(category) {
    if (!category || typeof category !== 'object') {
        showMessage(MESSAGES.CATEGORY_INVALID, 'error');
        return false;
    }

    if (!category.id || !category.name) {
        showMessage(MESSAGES.CATEGORY_INVALID, 'error');
        return false;
    }

    return true;
}