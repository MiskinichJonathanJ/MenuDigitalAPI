using Application.DataTransfers.Request.Dish;
using Domain.Entities;

namespace Application.Interfaces.IDish
{
    public interface IDishCommand
    {
        Task CreateDish(Dish dish);
        Task DeleteDish(Dish dish);
        Task UpdateDish(Dish dishEnDB, UpdateDishRequest dishActualizado);
    }
}
