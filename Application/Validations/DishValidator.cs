using Application.DataTransfers.Request.Dish;
using Application.Exceptions;
using Application.Interfaces.DishInterfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task ValidateUpdate(Guid idDish, UpdateDishRequest request)
        {
            ValidateCommon(request);
            await ValidateCategoryExists(request.Category);
            await ValidateDishNameUnique(request.Name, idDish);
        }

        public void ValidateCommon(DishBaseRequest  request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("El nombre del platillo no puede estar vacío");

            if (request.Name.Length > 255)
                throw new ArgumentException("El nombre del platillo no debe exceder los 256 caracteres");

            if (request.Price <= 0)
                throw new InvalidDishPriceException("El precio del platillo debe ser mayor a 0");
        }

        public async Task ValidateCategoryExists(int categoryId)
        {
            var category = await _query.GetCategoryById(categoryId) ?? throw new CategoryNotFoundException("La categoria no existe");
        }

        public async Task ValidateDishNameUnique(string name, Guid? id = null)
        {
            var dishes = await _query.GetAllDish(name: name);
            if (dishes.Any(d => id == null  || d.ID != id))
                throw new DishNameAlreadyExistsException("Ya existe un platillo con ese nombre");
        }
    }
}
