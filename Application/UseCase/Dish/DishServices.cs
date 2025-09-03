using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;
using Application.DataTransfers.Request;
using Application.DataTransfers.Response;
using Application.Interfaces.DishInterfaces;

namespace Application.UseCase.DishUse
{
    public class DishServices : IDishServices
    {
        public readonly IDishCommand _command;
        public readonly IDishQuery _query;
        public readonly IDishMapper _mapper;
        public readonly IDishValidator _validator;

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
        
        public Task DeleteDish(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<DishResponse>> GetAllDish(
            string? name = null,
            int? categoryId = null,
            bool onlyActive = true,
            string? sortByPrice = null
        )
        {
            var  result = await _query.GetAllDish(name,  categoryId,  onlyActive,  sortByPrice);
            return result.Select(d => _mapper.ToResponse(d)).ToList();
        }

        public Task<Dish> GetDishById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<DishResponse> UpdateDish(Guid id, DishRequest request)
        {
            await _validator.ValidateUpdate(id, request);
            var  dish = await _query.GetDishById(id);

            await _command.UpdateDish(dish, request);
            
            return _mapper.ToResponse(dish);
        }
    }
}
