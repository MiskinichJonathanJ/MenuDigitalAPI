using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Response;
using Application.DataTransfers.Response.Dish;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderItem;
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

        public OrderCreateResponse ToCreateResponse(Order order)
        {
            return new OrderCreateResponse
            {
                OrderNumber = order.Id,
                TotalMount = (Double)order.Price,
                CreatedDate = order.CreateDate
            };
        }

        public ICollection<OrderDetailsResponse> ToDetailsResponse(ICollection<Order> orders)
        {
            return [.. orders.Select(o =>  new OrderDetailsResponse
            {
                OrderNumber = o.Id,
                DeliveryTo = o.DeliveryTo,
                Notes = o.Notes,
                TotalMount = (Double)o.Price,
                Status = o.StatusNav == null ? null : new GenericResponse
                {
                    id = o.StatusNav.ID,
                    name = o.StatusNav.Name
                },
                DeliveryType = o.DeliveryTypeNav == null ? null : new GenericResponse
                {
                    id = o.DeliveryTypeNav.ID,
                    name = o.DeliveryTypeNav.Name
                },
                Items = o.Items == null ? null : [.. o.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    Dish = new DishShortResponse
                    {
                        Id = i.DishNav.ID,
                        Name = i.DishNav.Name,
                        Image = i.DishNav.ImageURL
                    },
                    Quantity = i.Quantity,
                    Notes = i.Notes,
                    Status = i.Status == null ? null : new GenericResponse
                    {
                        id = i.Status.ID,
                        name = i.Status.Name
                    }
                })],
                CreatedDate = o.CreateDate,
                UpdateDate = o.UpdateDate
            })];
        }
    }
}
