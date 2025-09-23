using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Response;
using Application.DataTransfers.Response.Dish;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderItem;
using Application.DataTransfers.Response.OrderResponse;
using Application.Interfaces.IOrder;
using Application.Validations.Helpers;
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
                OverallStatus = (int)OrderItemStatusFlow.OrderItemStatus.Pending,
                DeliveryType = request.Delivery.Id,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };
        }

        public OrderItem ToEntityItem(Items request, int  orderId)
        {
            return new OrderItem
            {
                Dish = request.Id,
                Order = orderId,
                Quantity = request.Quantity,
                Notes = request.Notes,
                CreatedDate = DateTime.UtcNow,
                Status = 1
            };
        }

        public ICollection<OrderItem> ToEntityItems(ICollection<Items> items)
        {
            return [.. items.Select(i => new OrderItem
            {
                Dish = i.Id,
                Order = 0,
                Quantity = i.Quantity,
                Notes = i.Notes,
                CreatedDate = DateTime.UtcNow,
                Status = 1
            })];   
        }

        public OrderCreateResponse ToCreateResponse(Order order)
        {
            return new OrderCreateResponse
            {
                OrderNumber = order.OrderId,
                TotalAmount = (Double)order.Price,
                CreatedAt = order.CreateDate
            };
        }

        public OrderDetailsResponse ToDetailsResponse(Order orders)
        {
            return new OrderDetailsResponse
            {
                OrderNumber = orders.OrderId,
                DeliveryTo = orders.DeliveryTo,
                Notes = orders.Notes,
                TotalAmount = (Double)orders.Price,
                Status = orders.StatusNav == null ? null : new GenericResponse
                {
                    id = orders.StatusNav.Id,
                    name = orders.StatusNav.Name
                },
                DeliveryType = orders.DeliveryTypeNav == null ? null : new GenericResponse
                {
                    id = orders.DeliveryTypeNav.Id,
                    name = orders.DeliveryTypeNav.Name
                },
                Items = orders.Items == null ? null : [.. orders.Items.Select(i => new OrderItemResponse
                {
                    Id = i.OrderItemId,
                    Dish = new DishShortResponse
                    {
                        Id = i.DishNav?.DishId ?? Guid.Empty,
                        Name = i.DishNav?.Name ?? "Sin Nombre",
                        Image = i.DishNav?.ImageUrl ?? "No image"
                    },
                    Quantity = i.Quantity,
                    Notes = i.Notes,
                    Status = i.StatusNav == null ? null : new GenericResponse
                    {
                        id = i.StatusNav.Id,
                        name = i.StatusNav.Name
                    }
                })],
                CreatedAt = orders.CreateDate,
                UpdatedAt = orders.UpdateDate
            };
        }

        public OrderUpdateResponse ToUpdateResponse(Order order)
        {
            return new OrderUpdateResponse
            {
                OrderNumber = order.OrderId,
                TotalAmount = (double)order.Price,
                UpdatedAt = order.UpdateDate
            };
        }
    }
}
