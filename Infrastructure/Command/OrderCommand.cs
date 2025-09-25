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
        private static void UpdateOrderOverallStatus(Order order)
        {
            if (order.Items.Any(i => i.Status == (int)OrderItemStatus.Preparing))
            {
                order.OverallStatus = (int)OrderItemStatus.Preparing;
            }
            else if (order.Items.All(i => i.Status == (int)OrderItemStatus.Ready))
            {
                order.OverallStatus = (int)OrderItemStatus.Ready;
            }
            else if (order.Items.All(i => i.Status == (int)OrderItemStatus.Delivered))
            {
                order.OverallStatus = (int)OrderItemStatus.Delivered;
            }
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

            var orderItem = order.Items.FirstOrDefault(item => item.OrderItemId == itemId)
                ?? throw new OrderItemNotFoundException();

            var  newStatus  = (OrderItemStatusFlow.OrderItemStatus)request.Status;
            if (!OrderItemStatusFlow.CanTransition((OrderItemStatusFlow.OrderItemStatus)orderItem.Status, newStatus))
                throw new InvalidOrderStatusTransitionException();
            orderItem.Status = (int)newStatus;

            UpdateOrderOverallStatus(order);

            order.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrder(ICollection<OrderItem> request, long id, decimal price)
        {
            var order = await GetOrderWithItemsNotClosedAsync(id);

            var requestDishes = request.Select(r => r.Dish).ToList();

            var existingItems = await _context.OrderItem
                .Where(oi => oi.Order == id && requestDishes.Contains(oi.Dish))
                .ToListAsync();

            var existingItemsDict = existingItems.ToDictionary(item => item.Dish);
            var itemsToAdd = new List<OrderItem>();

            foreach (var itemRequest in request)
            {
                if (existingItemsDict.TryGetValue(itemRequest.Dish, out var existingItem))
                    UpdateExistingItem(existingItem, itemRequest);
                else
                {
                    itemRequest.Order = id;
                    itemsToAdd.Add(itemRequest);
                }
            }

            if (itemsToAdd.Count != 0)
                await _context.OrderItem.AddRangeAsync(itemsToAdd);
            order.UpdateDate = DateTime.UtcNow;
            order.Price = price;

            await _context.SaveChangesAsync();
            return order;
        }
    }
}