using Application.DataTransfers.Request.Dish;
using Application.Exceptions;
using Application.Exceptions.CategoryException;
using Application.Exceptions.DishException;
using Application.Interfaces.IDish;

namespace Application.Validations
{
    public class DishValidator : IDishValidator
    {
        private readonly IDishQuery _query;

        public DishValidator(IDishQuery query)
        {
            _query = query;
        }

        public async Task ValidateCreate(DishBaseRequest request)
        {
            ValidateCommon(request);
            await ValidateCategoryExists(request.Category);
            await ValidateDishNameUnique(request.Name);
        }

        public async Task ValidateUpdate(Guid idDish, DishUpdateRequest request)
        {
            ValidateCommon(request);
            await ValidateCategoryExists(request.Category);
            await ValidateDishNameUnique(request.Name, idDish);
        }

        public void ValidateCommon(DishBaseRequest request)
        {
            if (request == null)
                throw new RequestNullException();
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new DishNameIsNullException();

            if (request.Name.Length > 255)
                throw new DishNameTooLongException();

            if (request.Price <= 0)
                throw new InvalidDishPriceException();
        }

        public async Task ValidateCategoryExists(int categoryId)
        {
           if (await _query.GetCategoryById(categoryId) == null)
            throw new CategoryNotFoundException();
        }

        public async Task ValidateDishNameUnique(string name, Guid? id = null)
        {
            var dishes = await _query.GetAllDish(name: name);
            if (dishes.Any(d => id == null  || d.DishId != id))
                throw new DishNameAlreadyExistsException();
        }
    }
}
