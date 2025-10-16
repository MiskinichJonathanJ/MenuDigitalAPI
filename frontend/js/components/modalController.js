export function initModalControls() {
    const modal = document.getElementById('dishModal');
    if (!modal) return;

    modal.addEventListener('click', (e) => {
        const btn = e.target.closest('button[data-action]');
        if (!btn) return;
        const action = btn.dataset.action;
        const qtyInput = modal.querySelector('.dish-quantity-input');
        if (!qtyInput) return;

        let v = parseInt(qtyInput.value, 10);
        if (Number.isNaN(v)) v = 1;

        if (action === 'increase') v = Math.min(99, v + 1);
        else if (action === 'decrease') v = Math.max(1, v - 1);
        qtyInput.value = String(v);
        qtyInput.dispatchEvent(new Event('input', { bubbles: true }));
    });

    modal.addEventListener('input', (e) => {
        const el = e.target;
        if (!el.classList || !el.classList.contains('dish-quantity-input')) return;
        let v = parseInt(el.value, 10);
        if (Number.isNaN(v) || v < 1) v = 1;
        if (v > 99) v = 99;
        if (String(v) !== el.value) el.value = String(v);
    });

    modal.addEventListener('keydown', (e) => {
        const el = e.target;
        if (!el.classList || !el.classList.contains('dish-quantity-input')) return;
        if (e.key === 'ArrowUp') {
            e.preventDefault();
            const v = Math.min(99, (parseInt(el.value, 10) || 1) + 1);
            el.value = String(v);
            el.dispatchEvent(new Event('input', { bubbles: true }));
        } else if (e.key === 'ArrowDown') {
            e.preventDefault();
            const v = Math.max(1, (parseInt(el.value, 10) || 1) - 1);
            el.value = String(v);
            el.dispatchEvent(new Event('input', { bubbles: true }));
        }
    });

    modal.addEventListener('shown.bs.modal', () => {
        const qtyInput = modal.querySelector('.dish-quantity-input');
        if (qtyInput) qtyInput.focus();
    });
}