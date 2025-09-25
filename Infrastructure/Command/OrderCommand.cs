using Application.DataTransfers.Request.Order;
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
        private async Task<Order> GetOrderWithItemsAsync(long orderId)
        {
            return await _context.Order
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new OrderNotFoundException();
        }
        private async Task<Order> GetOrderWithItemsNotClosedAsync(long orderId)
        {
            var closedStatuses = new[] {  (int)OrderItemStatus.Closed  };

            return await _context.Order
                .Where(o => o.OrderId == orderId && !closedStatuses.Contains(o.OverallStatus))
                .FirstOrDefaultAsync()
                ?? throw new OrderNotFoundException(); 
        }
        private static void UpdateExistingItem(OrderItem existingItem, OrderItem itemRequest)
        {
            existingItem.Notes = itemRequest.Notes;
            existingItem.Quantity = itemRequest.Quantity;
        }
        private async Task UpdateOrderOverallStatus(Order orderUpd)
        {
            ArgumentNullException.ThrowIfNull(orderUpd);

            var minStatus = await _context.OrderItem
                .Where(oi => oi.Order == orderUpd.OrderId)
                .MinAsync(oi => (int?)oi.Status);

            if (!minStatus.HasValue) return;

            orderUpd.OverallStatus = minStatus.Value;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _context.Order.AddAsync(order);

            await _context.SaveChangesAsync();
            return order;
        }
        
        public async Task<Order> UpdateStatusItemOrder(long orderId, long itemId, OrderItemUpdateRequest request)
        {
            var order = await GetOrderWithItemsAsync(orderId);

            var orderItem = await _context.OrderItem
                .FirstOrDefaultAsync(oi => oi.OrderItemId == itemId && oi.Order == orderId)
                ?? throw new OrderItemNotFoundException();

            var  newStatus  = (OrderItemStatusFlow.OrderItemStatus)request.Status;
            if (!OrderItemStatusFlow.CanTransition((OrderItemStatusFlow.OrderItemStatus)orderItem.Status, newStatus))
                throw new InvalidOrderStatusTransitionException();
            orderItem.Status = (int)newStatus;

            await UpdateOrderOverallStatus(order);

            order.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrder(ICollection<OrderItem> request, long id, decimal newPrice)
        {
            var order = await GetOrderWithItemsNotClosedAsync(id);

            var requestDishes = request.Select(r => r.Dish).ToList();

            var existingItems = await _context.OrderItem
                .Where(oi => oi.Order == id && requestDishes.Contains(oi.Dish))
                .ToDictionaryAsync(oi => oi.Dish);

            var itemsToAdd = new List<OrderItem>();

            foreach (var itemRequest in request)
            {
                if (existingItems.TryGetValue(itemRequest.Dish, out var existingItem))
                    UpdateExistingItem(existingItem, itemRequest);
                else
                { 
                    itemRequest.Order = id;
                    itemsToAdd.Add(itemRequest);
                }
            }

            if (itemsToAdd.Count > 0)
                await _context.OrderItem.AddRangeAsync(itemsToAdd);

            order.UpdateDate = DateTime.UtcNow;
            order.Price = newPrice;

            await _context.SaveChangesAsync();
            return order;
        }
    }
}