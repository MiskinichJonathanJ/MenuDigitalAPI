using Application.DataTransfers.Request.OrderItem;
using Application.Exceptions.OrderException;
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
        public async Task<Order> CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request)
        {
            var order = await _context.Orders.Where(o => o.Id == orderId)
                .Include(o => o.Items)
                .FirstOrDefaultAsync() ?? throw new InvalidOrderIdException();
            var orderItem = await _context.OrderItems.Where(oi => oi.Id == itemId && oi.OrderId == orderId).FirstOrDefaultAsync() ?? throw new OrderItemNotFoundException();

            var  newStatus  = (OrderItemStatusFlow.OrderItemStatus)request.Status;

            if (!OrderItemStatusFlow.CanTransition((OrderItemStatusFlow.OrderItemStatus)orderItem.StatusId, newStatus))
                throw new InvalidOrderStatusTransitionException();

            orderItem.StatusId = (int)newStatus;

            if (order.Items.All(i => i.StatusId == (int)OrderItemStatus.Preparing))
                order.OverallStatusID = (int)OrderItemStatus.Preparing;

            else if (order.Items.All(i => i.StatusId == (int)OrderItemStatus.Ready))
                order.OverallStatusID = (int)OrderItemStatus.Ready;

            else if (order.Items.All(i => i.StatusId == (int)OrderItemStatus.Delivered))
                order.OverallStatusID = (int)OrderItemStatus.Delivered;

            order.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order;
        }
    }
}