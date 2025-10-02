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

            modalContent.querySelector('.add-to-cart-modal-btn').addEventListener('click', (e) => {
                const id = e.target.dataset.dishId;
                CartService.addToCart(id);

                const modal = bootstrap.Modal.getInstance(document.getElementById('dishModal'));
                modal.hide();
            });

            new bootstrap.Modal(document.getElementById('dishModal')).show();
        } catch (err) {
            showError("Error al mostrar detalles del plato");
        }
    }
};
export { DishModal };