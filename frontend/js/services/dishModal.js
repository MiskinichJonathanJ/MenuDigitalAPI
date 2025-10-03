import { DishService } from '../services/dishService.js';
import { showError } from '../utils/helpers.js';
import { appState } from '../store.js';
import { CartService } from './cartService.js';
import { modalDishHTML } from '../components/modal.js';

const DishModal = {
    async show(dishId) {
        try {
            let dish = appState.dishes.find(d => d.id.toString() === dishId);
            
            if (!dish) dish = await DishService.getDishById(dishId);

            const modalContent = document.getElementById('dish-modal-content');
            const modalTitle = document.getElementById('dishModalLabel');

            if (!modalContent || !modalTitle) return;

            modalTitle.textContent = dish.name;
            modalContent.innerHTML = modalDishHTML(dish);

            const addBtn = modalContent.querySelector('.add-to-cart-modal-btn');
            if (addBtn) {
                addBtn.addEventListener('click', (e) => {
                    const id = e.currentTarget.dataset.dishId;
                    const qtyInput = modalContent.querySelector('.dish-quantity-input');
                    const notesInput = modalContent.querySelector('.dish-notes-input');

                    const quantity = qtyInput ? parseInt(qtyInput.value, 10) : 1;
                    const notes = notesInput ? String(notesInput.value || '').trim() : '';

                    if (!Number.isInteger(quantity) || quantity < 1) {
                        showError('La cantidad debe ser un número entero mayor o igual a 1');
                        return;
                    }

                    if (!dish.isActive) {
                        showError('No se puede agregar un plato que no está disponible');
                        return;
                    }

                    CartService.addToCart({ id, quantity, notes });

                    const modal = bootstrap.Modal.getInstance(document.getElementById('dishModal'));
                    if (modal) modal.hide();
                });
            }

            new bootstrap.Modal(document.getElementById('dishModal')).show();
        } catch (err) {
            showError("Error al mostrar detalles del plato");
        }
    }
};
export { DishModal };