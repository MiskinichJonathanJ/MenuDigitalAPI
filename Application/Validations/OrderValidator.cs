using Application.DataTransfers.Request.Order;
using Application.Interfaces.IOrder;

namespace Application.Validations
{
    public class OrderValidator : IOrderValidator
    {
        public readonly IOrderQuery _orderQuery;
        public OrderValidator(IOrderQuery orderQuery)
        {
            _orderQuery = orderQuery;
        }
        public Task ValidateCreateOrder(OrderRequest orderCreate)
        {
            if (orderCreate.Items == null || !orderCreate.Items.Any())
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
    }
}
