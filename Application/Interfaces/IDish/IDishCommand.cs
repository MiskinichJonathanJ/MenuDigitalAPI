using Application.DataTransfers.Request.Dish;
using Domain.Entities;

namespace Application.Interfaces.IDish
{
    public interface IDishCommand
    {
        Task<Dish> CreateDish(Dish dish);
        Task DeleteDish(Guid dishId);
        Task<Dish> UpdateDish(Guid id, DishUpdateRequest dishActualizado);
    }
}
