using Application.DataTransfers.Request.Dish;
using Domain.Entities;

namespace Application.Interfaces.IDish
{
    public interface IDishCommand
    {
        Task<Dish> CreateDish(Dish dish);
        Task DeleteDish(Guid dishId);
        Task UpdateDish(Dish dishEnDB, UpdateDishRequest dishActualizado);
    }
}
