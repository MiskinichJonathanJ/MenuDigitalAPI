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
            await  ValidateCommon(request);

            var dishConNombre = await _query.GetAllDish(name: request.Name);
            if (dishConNombre.Count != 0)
                throw new DishNameAlreadyExistsException("Ya existe un platillo con ese nombre");
        }

        public async Task ValidateUpdate(Guid idDish, UpdateDishRequest request)
        {
            if(await _query.GetDishById(idDish) == null)
                throw new DishNotFoundException("El platillo no existe");

            await ValidateCommon(request);

            var dishConNombre = await _query.GetAllDish(name: request.Name);
            if (dishConNombre.Any(d => d.ID != idDish))
                throw new DishNameAlreadyExistsException("Ya existe un platillo con ese nombre");
        }

        public async Task ValidateCommon(DishBaseRequest  request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("El nombre del platillo no puede estar vacío");

            if (request.Name.Length > 255)
                throw new ArgumentException("El nombre del platillo no debe exceder los 256 caracteres");

            if (request.Price <= 0)
                throw new InvalidDishPriceException("El precio del platillo debe ser mayor a 0");

            var category = await _query.GetCategoryById(request.Category);
            if (category == null)
                throw new CategoryNotFoundException("La categoria no existe");
        }
    }
}
