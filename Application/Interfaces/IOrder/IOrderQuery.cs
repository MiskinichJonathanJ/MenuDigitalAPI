using Application.DataTransfers.Request.Order;
using Domain.Entities;

namespace Application.Interfaces.IOrder
{
    public interface IOrderQuery
    {
        Task<ICollection<Dish>> GetAllDishesOrder(ICollection<ItemRequest> orderItems);
    }
}
