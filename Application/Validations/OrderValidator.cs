using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.Interfaces.IOrder;
using Application.Validations.Helpers;

namespace Application.Validations
{
    public class OrderValidator : IOrderValidator
    {
        public Task ValidateCreateOrder(OrderRequest orderCreate)
        {
            if (orderCreate.Items == null || orderCreate.Items.Count == 0)
                throw new Exception("El pedido debe contener al menos un plato.");
            foreach (var item  in  orderCreate.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("La cantidad de cada plato debe ser mayor a cero.");
                if (item.Id == Guid.Empty)
                    throw new Exception("El id del plato es inválido.");
            }

            if  (orderCreate.Delivery.Id == 1 && string.IsNullOrEmpty(orderCreate.Delivery.To))
                throw new Exception("La dirección de entrega es obligatoria para el tipo de entrega a domicilio.");
            return Task.CompletedTask;
        }

        public Task ValidateGetAllOrders(DateTime? from = null, DateTime? to = null, int? status = null)
        {
            if  (from != null)
            {
                if (from > DateTime.Now)
                    throw new Exception("La fecha 'desde' no puede ser mayor a la fecha actual.");
                if (to != null && from > to)
                    throw new Exception("La fecha 'desde' no puede ser mayor a la fecha 'hasta'.");
            }

            if (status != null)
            {
                var validStatuses = new List<int> { 1, 2, 3, 4 };
                if (!validStatuses.Contains(status.Value))
                    throw new Exception("El estado proporcionado no es válido.");
            }

            return Task.CompletedTask;
        }

        public Task ValidateGetOrderById(int orderId)
        {
            if (orderId <= 0)
                throw new Exception("El ID del pedido debe ser un número positivo.");
            return Task.CompletedTask;
        }

        public Task ValidateUpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request)
        {
            if (orderId <=  0 || itemId <= 0)
                throw new Exception("Los IDs deben de ser positivos.");
            if(Enum.IsDefined(typeof(OrderItemStatusFlow.OrderItemStatus), request.Status))
                throw new Exception("El estado proporcionado no es válido.");

            return Task.CompletedTask;
        }

        public Task ValidateUpdateOrder(OrderUpdateRequest request)
        {
            if(request.Items.Count == 0)
                throw new Exception("El pedido debe contener al menos un plato.");
            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("La cantidad de cada plato debe ser mayor a cero.");
                if (item.Id == Guid.Empty)
                    throw new Exception("El id del plato es inválido.");
            }

            return Task.CompletedTask;
        }
    }
}
