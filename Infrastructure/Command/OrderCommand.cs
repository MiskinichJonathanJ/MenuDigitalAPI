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
        private async Task<Order> GetOrderWithItemsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new InvalidOrderIdException();
        }
        private static void UpdateExistingItem(OrderItem existingItem, OrderItem itemRequest)
        {
            existingItem.Notes = itemRequest.Notes;
            existingItem.Quantity = itemRequest.Quantity;
        }
        private static void UpdateOrderOverallStatus(Order order)
        {
            if (order.Items.Any(i => i.StatusId == (int)OrderItemStatus.Preparing))
            {
                order.OverallStatusID = (int)OrderItemStatus.Preparing;
            }
            else if (order.Items.All(i => i.StatusId == (int)OrderItemStatus.Ready))
            {
                order.OverallStatusID = (int)OrderItemStatus.Ready;
            }
            else if (order.Items.All(i => i.StatusId == (int)OrderItemStatus.Delivered))
            {
                order.OverallStatusID = (int)OrderItemStatus.Delivered;
            }
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();
            return order;
        }
        
        public async Task<Order> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request)
        {
            var order = await GetOrderWithItemsAsync(orderId);

            var orderItem = order.Items.FirstOrDefault(item => item.Id == itemId)
                ?? throw new OrderItemNotFoundException();

            var  newStatus  = (OrderItemStatusFlow.OrderItemStatus)request.Status;
            if (!OrderItemStatusFlow.CanTransition((OrderItemStatusFlow.OrderItemStatus)orderItem.StatusId, newStatus))
                throw new InvalidOrderStatusTransitionException();
            orderItem.StatusId = (int)newStatus;

            UpdateOrderOverallStatus(order);

            order.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrder(ICollection<OrderItem> request, int id, decimal price)
        {
            var order = await GetOrderWithItemsAsync(id);
            var existingItemsDict = order.Items.ToDictionary(item => item.DishId);

            foreach (var itemRequest in request)
            {
                if (existingItemsDict.TryGetValue(itemRequest.DishId, out var existingItem))
                    UpdateExistingItem(existingItem, itemRequest);
                else
                    order.Items.Add(itemRequest);
            }

            order.UpdateDate = DateTime.UtcNow;
            order.Price = price;
            await _context.SaveChangesAsync();
            return order;
        }
    }
}