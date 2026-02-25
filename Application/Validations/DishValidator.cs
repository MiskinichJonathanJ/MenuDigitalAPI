using FluentValidation;
using Application.DataTransfers.Request.Dish;
using Application.Interfaces.IDish;

namespace Application.Validations
{
    public class DishValidator : AbstractValidator<DishBaseRequest>
    {
        private readonly IDishQuery _query;

        public DishValidator(IDishQuery query)
        {
            _query = query;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .MaximumLength(255).WithMessage("El nombre excede los 255 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            RuleFor(x => x.Category)
                .MustAsync(CategoryExists).WithMessage("La categoría no existe.");

            // Nota: La validación de unicidad de nombre requiere el ID, es mejor manejarla
            // en un validador específico para Update/Create heredando de este o en la lógica de negocio.
        }

        private async Task<bool> CategoryExists(int categoryId, CancellationToken cancellationToken)
        {
            return await _query.GetCategoryById(categoryId) != null;
        }
    }
}