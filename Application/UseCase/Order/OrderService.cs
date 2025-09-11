using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderResponse;
using Application.Interfaces.IOrder;

namespace Application.UseCase.OrderUse
{
    public class OrderService : IOrderService
    {
        public  readonly IOrderCommand _command;
        public  readonly IOrderValidator _validator;
        public  readonly IOrderMapper _mapper;
        private readonly IOrderQuery _query;

        public OrderService(IOrderCommand command, IOrderValidator validator, IOrderMapper mapper, IOrderQuery query)
        {
            _command = command;
            _validator = validator;
            _mapper = mapper;
            _query = query;
        }

        public async Task<OrderCreateResponse> CreateOrder(OrderRequest orderCreate)
        {
            await _validator.ValidateCreateOrder(orderCreate);
            var dishes = await _query.GetAllDishesOrder(orderCreate.Items);
            if (dishes.Count != orderCreate.Items.Count)
                throw new Exception("Uno o más platos no existen.");

            double totalPrice = 0;
            foreach (var item in orderCreate.Items)
            {
                var dish = dishes.First(d => d.ID == item.Id);
                totalPrice += (double)(dish.Price * item.Quantity);
            }

            var orderEntity = _mapper.ToEntity(orderCreate);
            orderEntity.Price = (decimal)totalPrice;

            orderEntity.Items = _mapper.ToEntityItems(orderCreate.Items, orderEntity.Id);

            await _command.CreateOrder(orderEntity);

            return _mapper.ToCreateResponse(orderEntity);
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

        public async Task<OrderUpdateResponse> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request)
        {
            await _validator.ValidateUpdateStatusItemOrder(orderId, itemId,  request);
            var order = await _command.UpdateStatusItemOrder(orderId, itemId, request);
            return _mapper.ToUpdateResponse(order);
        }
    }
}
