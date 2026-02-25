using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response.Dish;
using Application.Exceptions.DishException;
using Application.Interfaces.IDish;
using Domain.Entities;
using FluentValidation;


namespace Application.UseCase.DishUse
{
    public class DishService : IDishServices
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly IDishMapper _mapper;
        private readonly IValidator<DishBaseRequest> _validator;

        public DishService(IDishCommand command, IDishQuery query, IDishMapper mapper, IValidator<DishBaseRequest> validator)
        {
            _command = command;
            _query = query;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<DishResponse> CreateDish(DishRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var dish = _mapper.ToEntity(request);

            var dishCreated = await _command.CreateDish(dish);
            return _mapper.ToResponse(dishCreated);
        }
        
        public async Task<DishResponse> DeleteDish(Guid id)
        {
            var dish = await GetDishOrThrow(id);
            await _command.DeleteDish(id);
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

        public async Task<DishResponse> UpdateDish(Guid id, DishUpdateRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var dish = await _command.UpdateDish(id, request);
            
            return _mapper.ToResponse(dish);
        }

        private async Task<Dish> GetDishOrThrow(Guid id)
        {
            var dish = await _query.GetDishById(id);

            return dish ?? throw new DishNotFoundException();
        }
    }
}
