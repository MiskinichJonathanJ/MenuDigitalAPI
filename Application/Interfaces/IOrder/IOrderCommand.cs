using Application.DataTransfers.Request.OrderItem;
using Domain.Entities;

namespace Application.Interfaces.IOrder
{
    public interface IOrderCommand
    {
        Task CreateOrder(Order order);
        Task<Order> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request);
    }
}
