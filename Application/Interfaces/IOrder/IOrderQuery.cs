using Application.DataTransfers.Request.Order;
using Domain.Entities;

namespace Application.Interfaces.IOrder
{
    public interface IOrderQuery
    {
        Task<ICollection<Dish>> GetAllDishesOrder(ICollection<Items> orderItems);
        Task<ICollection<Order>> GetAllOrders(DateTime? desde = null, DateTime? hasta = null, int? statusId = null);
        Task<Order> GetOrderById(int orderId);
    }
}
