using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Response.OrderResponse;
using Application.Interfaces.IOrder;
using Domain.Entities;

namespace Application.Mappers.OrderMap
{
    public class OrderMapper : IOrderMapper
    {
        public Order ToEntity(OrderRequest request)
        {
            return new Order
            {
                DeliveryTo = request.Delivery.To ?? (request.Delivery.Id == 2 ? "Take away" : "Dine In"),
                Notes = request.Notes,
                Price = 0,
                OverallStatusID = 1, 
                DeliveryTypeID = request.Delivery.Id, 
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };
        }

        public OrderItem ToEntityItem(ItemRequest request, int  orderId)
        {
            return new OrderItem
            {
                DishId = request.Id,
                OrderId = orderId,
                Quantity = request.Quantity,
                Notes = request.Notes,
                CreatedDate = DateTime.UtcNow,
                StatusId = 1
            };
        }

        public ICollection<OrderItem> ToEntityItems(ICollection<ItemRequest> items, int orderId)
        {
            return [.. items.Select(i => new OrderItem
            {
                DishId = i.Id,
                OrderId = orderId,
                Quantity = i.Quantity,
                Notes = i.Notes,
                CreatedDate = DateTime.UtcNow,
                StatusId = 1
            })];   
        }

        public OrderResponse ToResponse(Order order, double price)
        {
            return new OrderResponse
            {
                OrderNumber = order.Id,
                TotalMount = price,
                CreatedDate = order.CreateDate
            };
        }
    }
}
