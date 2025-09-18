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
                OverallStatusID = 1,
                DeliveryTypeID = 1,
                Items = {
                    new OrderItem { StatusId= 1, DishId = Guid.NewGuid(), OrderId = 0}
                },
                CreateDate = DateTime.Now,
            };
        }
        protected async Task<Order> CreateValidOrderOnDB()
        {
            var order = CreateOrderValid();

            _context.Orders.Add(order);
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
                    OrderId = 0,
                    DishId = Guid.NewGuid(),
                    Quantity = i + 1, 
                    Notes = $"Test Item {i + 1}",
                    CreatedDate = DateTime.UtcNow,
                    StatusId = (int)OrderItemStatus.Pending
                });
            }

            var order = new Order
            {
                DeliveryTo = "Test Address Multiple Items",
                Notes = $"Test Order with {itemCount} Items",
                Price = itemCount * 50m,
                OverallStatusID = (int)OrderItemStatus.Pending, 
                DeliveryTypeID = 1,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Items = items
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await _context.Orders
                .Include(o => o.Items)
                .FirstAsync(o => o.Id == order.Id);
        }

       

        
    }
}
