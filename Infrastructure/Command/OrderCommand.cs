using Application.DataTransfers.Request.OrderItem;
using Application.Interfaces.IOrder;
using Application.Validations.Helpers;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static Application.Validations.Helpers.OrderItemStatusFlow;

namespace Infrastructure.Command
{
    public class OrderCommand : IOrderCommand
    {
        private readonly  AppDbContext _context;
        public OrderCommand(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();
        }

        public Task CreateOrderItems(ICollection<OrderItem> orderItems)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request)
        {
            var order = await _context.Orders.Where(o => o.Id == orderId).FirstOrDefaultAsync() ?? throw new Exception("La orden no existe");
            var orderItem = await _context.OrderItems.Where(oi => oi.Id == itemId && oi.OrderId == orderId).FirstOrDefaultAsync() ?? throw new Exception("El item no existe en la orden");

            var  newStatus  = (OrderItemStatusFlow.OrderItemStatus)request.Status;

            if (!OrderItemStatusFlow.CanTransition((OrderItemStatusFlow.OrderItemStatus)orderItem.StatusId, newStatus))
                throw new Exception($"No se puede cambiar el estado de {(OrderItemStatusFlow.OrderItemStatus)orderItem.StatusId} a {newStatus}");

            orderItem.StatusId = (int)newStatus;

            if (order.Items.Any(i => i.StatusId == (int)OrderItemStatus.Preparing))
                order.OverallStatusID = (int)OrderItemStatus.Preparing;

            if (order.Items.All(i => i.StatusId == (int)OrderItemStatus.Ready))
                order.OverallStatusID = (int)OrderItemStatus.Ready;

            if (order.Items.All(i => i.StatusId == (int)OrderItemStatus.Delivered))
                order.OverallStatusID = (int)OrderItemStatus.Delivered;

            order.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return order;
        }
    }
}
