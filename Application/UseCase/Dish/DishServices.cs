using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response.Dish;
using Application.Exceptions;
using Application.Exceptions.DishException;
using Application.Interfaces.IDish;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DishUse
{
    public class DishServices : IDishServices
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly IDishMapper _mapper;
        private readonly IDishValidator _validator;

        public DishServices(IDishCommand command, IDishQuery query, IDishMapper mapper, IDishValidator validator)
        {
            _command = command;
            _query = query;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<DishResponse> CreateDish(DishRequest request)
        {
            await _validator.ValidateCreate(request);

            var dish = _mapper.ToEntity(request);

            await _command.CreateDish(dish);
            return _mapper.ToResponse(dish);
        }
        
        public async Task<DishResponse> DeleteDish(Guid id)
        {
            var dish = await GetDishOrThrow(id);
            await _command.DeleteDish(dish);
            return _mapper.ToResponse(dish);
        }

        public async Task<ICollection<DishResponse>> GetAllDish(
            string? name = null,
            int? categoryId = null,
            bool? onlyActive = null,
            string? sortByPrice = null
        )
        {
            var  result = await _query.GetAllDish(name,  categoryId,  onlyActive,  sortByPrice);
            return result.Select(d => _mapper.ToResponse(d)).ToList();
        }

        public async Task<DishResponse> GetDishById(Guid  id)
        {
            var dish = await GetDishOrThrow(id);
            return _mapper.ToResponse(dish);
        }

        public async Task<DishResponse> UpdateDish(Guid id, UpdateDishRequest request)
        {
            var dish = await GetDishOrThrow(id);
            await _validator.ValidateUpdate(id, request);

            await _command.UpdateDish(dish, request);
            
            return _mapper.ToResponse(dish);
        }

        private async Task<Dish> GetDishOrThrow(Guid id)
        {
            var dish = await _query.GetDishById(id);

            return dish ?? throw new DishNotFoundException();
        }
    }
}
