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
                OverallStatusID = (int)OrderItemStatusFlow.OrderItemStatus.Pending,
                DeliveryTypeID = request.Delivery.Id,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };
        }

        public OrderItem ToEntityItem(Items request, int  orderId)
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

        public ICollection<OrderItem> ToEntityItems(ICollection<Items> items)
        {
            return [.. items.Select(i => new OrderItem
            {
                DishId = i.Id,
                OrderId = 0,
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

        public OrderDetailsResponse ToDetailsResponse(Order orders)
        {
            return new OrderDetailsResponse
            {
                OrderNumber = orders.Id,
                DeliveryTo = orders.DeliveryTo,
                Notes = orders.Notes,
                TotalMount = (Double)orders.Price,
                Status = orders.StatusNav == null ? null : new GenericResponse
                {
                    id = orders.StatusNav.ID,
                    name = orders.StatusNav.Name
                },
                DeliveryType = orders.DeliveryTypeNav == null ? null : new GenericResponse
                {
                    id = orders.DeliveryTypeNav.ID,
                    name = orders.DeliveryTypeNav.Name
                },
                Items = orders.Items == null ? null : [.. orders.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    Dish = new DishShortResponse
                    {
                        Id = i.DishNav?.ID ?? Guid.Empty,
                        Name = i.DishNav?.Name ?? "Sin Nombre",
                        Image = i.DishNav?.ImageURL ?? "No image"
                    },
                    Quantity = i.Quantity,
                    Notes = i.Notes,
                    Status = i.Status == null ? null : new GenericResponse
                    {
                        id = i.Status.ID,
                        name = i.Status.Name
                    }
                })],
                CreatedDate = orders.CreateDate,
                UpdateDate = orders.UpdateDate
            };
        }

        public OrderUpdateResponse ToUpdateResponse(Order order)
        {
            return new OrderUpdateResponse
            {
                OrderNumber = order.Id,
                TotalMount = (double)order.Price,
                UpdatedDate = order.UpdateDate
            };
        }
    }
}
