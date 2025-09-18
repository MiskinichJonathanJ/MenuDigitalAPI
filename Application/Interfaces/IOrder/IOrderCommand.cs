using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Domain.Entities;

namespace Application.Interfaces.IOrder
{
    public interface IOrderCommand
    {
        Task<Order> CreateOrder(Order order);
        Task<Order> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request);
        Task<Order> UpdateOrder(ICollection<OrderItem> request, int id, decimal newPrice);
    }
}
