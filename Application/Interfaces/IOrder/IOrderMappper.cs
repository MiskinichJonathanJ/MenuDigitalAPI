using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderResponse;
using Domain.Entities;

namespace Application.Interfaces.IOrder
{
    public interface IOrderMapper
    {
        Order ToEntity(OrderRequest request);
        ICollection<OrderItem> ToEntityItems(ICollection<Items> items, int orderId);
        OrderCreateResponse ToCreateResponse(Order order);
        OrderItem ToEntityItem(Items request,  int orderId);
        OrderDetailsResponse ToDetailsResponse(Order orders);
        OrderUpdateResponse ToUpdateResponse(Order order);
    }
}
