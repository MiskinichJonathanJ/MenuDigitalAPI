using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Response.OrderResponse;
using Domain.Entities;

namespace Application.Interfaces.IOrder
{
    public interface IOrderMapper
    {
        Order ToEntity(OrderRequest request);
        ICollection<OrderItem> ToEntityItems(ICollection<ItemRequest> items, int orderId);
        OrderResponse ToResponse(Order order, double price);
        OrderItem ToEntityItem(ItemRequest request,  int orderId);
    }
}
