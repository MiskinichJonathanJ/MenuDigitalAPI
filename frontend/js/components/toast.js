export function toastHTML(message) {
    return `
    <div class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="polite">
      <div class="d-flex">
        <div class="toast-body">
          <i class="fas fa-check-circle me-2"></i>${message}
        </div>
        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
      </div>
    </div>
  `
}