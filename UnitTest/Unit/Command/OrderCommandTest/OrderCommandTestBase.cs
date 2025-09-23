using Application.DataTransfers.Request.OrderItem;
using Application.Exceptions.OrderException;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Command;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static Application.Validations.Helpers.OrderItemStatusFlow;

namespace UnitTest.Unit.Command.OrderCommandTest
{
    public class OrderCommandTestBase
    {
        protected readonly AppDbContext _context;
        protected readonly OrderCommand _orderCommand;

        public OrderCommandTestBase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _orderCommand = new OrderCommand(_context);
        }

        protected static Order CreateOrderValid()
        {
            return new Order
            {
                DeliveryTo = "Calle 123",
                Price = 1233.3M,
                OverallStatus = 1,
                DeliveryType = 1,
                Items = {
                    new OrderItem { Status= 1, Dish = Guid.NewGuid(), Order = 0}
                },
                CreateDate = DateTime.Now,
            };
        }
        protected async Task<Order> CreateValidOrderOnDB()
        {
            var order = CreateOrderValid();

            _context.Order.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        protected async Task<Order> CreateOrderWithMultipleItems(int itemCount)
        {
            var items = new List<OrderItem>();
            for (int i = 0; i < itemCount; i++)
            {
                items.Add(new OrderItem
                {
                    Order = 0,
                    Dish = Guid.NewGuid(),
                    Quantity = i + 1, 
                    Notes = $"Test Item {i + 1}",
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)OrderItemStatus.Pending
                });
            }

            var order = new Order
            {
                DeliveryTo = "Test Address Multiple Items",
                Notes = $"Test Order with {itemCount} Items",
                Price = itemCount * 50m,
                OverallStatus = (int)OrderItemStatus.Pending, 
                DeliveryType = 1,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Items = items
            };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return await _context.Order
                .Include(o => o.Items)
                .FirstAsync(o => o.OrderId == order.OrderId);
        }

       

        
    }
}
