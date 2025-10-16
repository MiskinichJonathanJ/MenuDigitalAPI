import { renderDishCards } from './dishCard.js';
import { showErrorState } from './loader.js';
import { modalDishHTML } from './modal.js';
import { showError } from '../utils/helpers.js';
import { initModalControls } from './modalController.js';

export function updateDishesUI(dishes, hasError = false) {
    const container = document.getElementById('dishes-container');
    const loadingElement = document.getElementById('loading-dishes');
    const noResultsElement = document.getElementById('no-dishes');

    if (!container) return;

    if (loadingElement) loadingElement.style.display = 'none';

    if (hasError) {
        showErrorState(container, {
            title: 'Error al cargar platos',
            message: 'Verifique la conexión con el servidor',
            retryAction: 'DishService.filterAndRenderDishes'
        });
        return;
    }

    if (!Array.isArray(dishes)) {
        showErrorState(container, {
            title: 'Error al cargar platos',
            message: 'Datos inválidos recibidos del servidor'
        });
        return;
    }

    if (dishes.length === 0) {
        container.innerHTML = '';
        if (noResultsElement) noResultsElement.classList.remove('d-none');
        return;
    }

    if (noResultsElement) noResultsElement.classList.add('d-none');

    container.innerHTML = renderDishCards(dishes);
}

export function showDishModal(dish, onAddToCart) {
    initModalControls();
    const modalElement = document.getElementById('dishModal');
    const modalContent = document.getElementById('dish-modal-content');
    const modalTitle = document.getElementById('dishModalLabel');

    if (!modalElement || !modalContent || !modalTitle) {
        showError('Error al mostrar el modal');
        return;
    }
    
    modalTitle.textContent = dish.name;
    modalContent.innerHTML = modalDishHTML(dish);

    setupModalAddButton(dish, onAddToCart);

    const bsModal = new bootstrap.Modal(modalElement);
    bsModal.show();
}

function setupModalAddButton(dish, onAddToCart) {
    const modalContent = document.getElementById('dish-modal-content');
    if (!modalContent) return;

    const addBtn = modalContent.querySelector('.add-to-cart-modal-btn');
    if (!addBtn) return;

    const newBtn = addBtn.cloneNode(true);
    addBtn.parentNode.replaceChild(newBtn, addBtn);

    newBtn.addEventListener('click', () => {
        const quantityInput = modalContent.querySelector('.dish-quantity-input');
        const notesInput = modalContent.querySelector('.dish-notes-input');

        const quantity = quantityInput ? parseInt(quantityInput.value, 10) : 1;
        const notes = notesInput ? notesInput.value.trim() : '';

        if (!Number.isInteger(quantity) || quantity < 1 || quantity > 99) {
            showError('La cantidad debe estar entre 1 y 99');
            return;
        }

        const success = onAddToCart({
            id: dish.id,
            quantity,
            notes
        });

        if (success) {
            const modalElement = document.getElementById('dishModal');
            const bsModal = bootstrap.Modal.getInstance(modalElement);
            if (bsModal) bsModal.hide();
        }
    });
}