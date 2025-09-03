using Application.DataTransfers.Request;
using Domain.Entities;

namespace Application.Interfaces.DishInterfaces
{
    public interface IDishCommand
    {
        Task CreateDish(Dish dish);
        Task DeleteDish(int id);
        Task UpdateDish(Dish dishEnDB, DishRequest dishActualizado);
    }
}
