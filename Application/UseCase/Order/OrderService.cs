using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderResponse;
using Application.Exceptions.OrderException;
using Application.Interfaces.IOrder;

namespace Application.UseCase.OrderUse
{
    public class OrderService : IOrderService
    {
        private readonly IOrderCommand _command;
        private readonly IOrderValidator _validator;
        private readonly IOrderMapper _mapper;
        private readonly IOrderQuery _query;

        public OrderService(IOrderCommand command, IOrderValidator validator, IOrderMapper mapper, IOrderQuery query)
        {
            _command = command;
            _validator = validator;
            _mapper = mapper;
            _query = query;
        }

        private async  Task<decimal> CalculatePriceOrder(ICollection<Items> orderItems)
        {
            var dishes = await _query.GetAllDishesOrder(orderItems);
            var dishesById = dishes.ToDictionary(d => d.ID);

            decimal totalPrice = 0m;
            foreach (var item in orderItems)
            {
                if (!dishesById.TryGetValue(item.Id, out var dish))
                    throw new DishNotAvailableException();

                totalPrice += dish.Price * item.Quantity;
            }

            return totalPrice;
        }

        public async Task<OrderCreateResponse> CreateOrder(OrderRequest orderCreate)
        {
            await _validator.ValidateCreateOrder(orderCreate);

            var orderEntity = _mapper.ToEntity(orderCreate);
            orderEntity.Price = await CalculatePriceOrder(orderCreate.Items);
            orderEntity.Items = _mapper.ToEntityItems(orderCreate.Items);

            var orderCreated = await _command.CreateOrder(orderEntity);

            return _mapper.ToCreateResponse(orderCreated);
        }

        public  async Task<ICollection<OrderDetailsResponse>> GetAllOrders(DateTime? desde = null, DateTime? hasta = null, int? statusId = null)
        {
            await _validator.ValidateGetAllOrders(desde, hasta, statusId);
            var orders = await _query.GetAllOrders(desde, hasta, statusId);
            return [..  orders.Select(o => _mapper.ToDetailsResponse(o))];
        }

        public async Task<OrderDetailsResponse> GetOrderById(int orderId)
        {
            await _validator.ValidateGetOrderById(orderId);
            var order = await _query.GetOrderById(orderId);
            return _mapper.ToDetailsResponse(order);
        }

        public async Task<OrderUpdateResponse> UpdateOrder(OrderUpdateRequest request, int id)
        {
            await _validator.ValidateUpdateOrder(request);

            var orderItemEntity = _mapper.ToEntityItems(request.Items);
            decimal newPrice = await CalculatePriceOrder(request.Items); 
            var order = await _command.UpdateOrder(orderItemEntity, id, newPrice);
            

            return _mapper.ToUpdateResponse(order);
        }

        public async Task<OrderUpdateResponse> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request)
        {
            await _validator.ValidateUpdateStatusItemOrder(orderId, itemId,  request);
            var order = await _command.UpdateStatusItemOrder(orderId, itemId, request);
            return _mapper.ToUpdateResponse(order);
        }
    }
}
