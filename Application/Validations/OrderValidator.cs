using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.Exceptions.OrderException;
using Application.Exceptions.StatusException;
using Application.Interfaces.IOrder;
using Application.Validations.Helpers;

namespace Application.Validations
{
    public class OrderValidator : IOrderValidator
    {
        public Task ValidateCreateOrder(OrderRequest orderCreate)
        {
            if (orderCreate.Items == null || orderCreate.Items.Count == 0)
                throw new EmptyOrderItemsException();
            foreach (var item  in  orderCreate.Items)
            {
                if (item.Quantity <= 0)
                    throw new InvalidItemQuantityException();
                if (item.Id == Guid.Empty)
                    throw new InvalidIdItemException();
            }

            if  (orderCreate.Delivery.Id == 1 && string.IsNullOrWhiteSpace(orderCreate.Delivery.To))
                throw new MissingAdrresDeliveryException();
            return Task.CompletedTask;
        }

        public Task ValidateGetAllOrders(DateTime? from = null, DateTime? to = null, int? status = null)
        {
            if  (from != null)
            {
                if (to != null && from > to)
                    throw new InvalidDateOrderException();
            }

            if (status != null && !Enum.IsDefined(typeof(OrderItemStatusFlow.OrderItemStatus), status))
                throw new StatusNotFoundException();

            return Task.CompletedTask;
        }

        public Task ValidateGetOrderById(int orderId)
        {
            if (orderId <= 0)
                throw new InvalidOrderIdException();
            return Task.CompletedTask;
        }

        public Task ValidateUpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request)
        {
            if (orderId <=  0 || itemId <= 0)
                throw new InvalidOrderIdException();
            if(!Enum.IsDefined(typeof(OrderItemStatusFlow.OrderItemStatus), request.Status))
                throw new StatusNotFoundException();

            return Task.CompletedTask;
        }

        public Task ValidateUpdateOrder(OrderUpdateRequest request)
        {
            if(request.Items.Count == 0)
                throw new EmptyOrderItemsException();
            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                    throw new InvalidItemQuantityException();
                if (item.Id == Guid.Empty)
                    throw new InvalidIdItemException();
            }

            return Task.CompletedTask;
        }
    }
}
