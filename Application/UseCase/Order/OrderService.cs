using Application.DataTransfers.Request.Order;
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

            var totalPrice = dishes.
                Join(orderCreate.Items,
                    d => d.ID,
                    i => i.Id,
                    (d, i) => (double)d.Price * i.Quantity)
                   .Sum();

            var orderEntity = _mapper.ToEntity(orderCreate);

            orderEntity.Items = _mapper.ToEntityItems(orderCreate.Items, orderEntity.Id);

            await _command.CreateOrder(orderEntity);

            return _mapper.ToCreateResponse(orderEntity);
        }

        public  async Task<ICollection<OrderDetailsResponse>> GetAllOrders(DateTime? desde = null, DateTime? hasta = null, int? statusId = null)
        {
            await _validator.ValidateGetAllOrders(desde, hasta, statusId);
            var orders = await _query.GetAllOrders(desde, hasta, statusId);
            return _mapper.ToDetailsResponse(orders);
        }
    }
}
