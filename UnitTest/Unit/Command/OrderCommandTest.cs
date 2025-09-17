using Application.DataTransfers.Request.OrderItem;
using Application.Exceptions.OrderException;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Command;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static Application.Validations.Helpers.OrderItemStatusFlow;

namespace UnitTest.Unit.Command
{
    public class OrderCommandTest
    {
        private readonly AppDbContext _context;
        private readonly OrderCommand _orderCommand;

        public OrderCommandTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _orderCommand = new OrderCommand(_context);
        }

        private  static Order CreateOrderValid()
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
        private async Task<Order> CreateValidOrderOnDB()
        {
            var order = CreateOrderValid();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        private async Task<Order> CreateOrderWithMultipleItems(int itemCount)
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

        [Fact]
        public async Task CreateOrder_WithValidOrder_ReturnsOrderWithGeneratedID()
        {
            // ARRANGE
            var order = CreateOrderValid();
            var countOrders = await _context.Orders.CountAsync();

            //  ACT
            var result = await _orderCommand.CreateOrder(order);

            // ASSERT
            Assert.NotNull(result);
            result.Id.Should().Be(order.Id);
            result.Items.First().OrderId.Should().Be(result.Id);

            var countAfter = await _context.Orders.CountAsync();
            countAfter.Should().Be(countOrders + 1);

            var savedOrder = await _context.Orders.FindAsync(result.Id);
            savedOrder.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateStatusItemOrder_ValidTransition_UpdatesItemStatusSuccessfully()
        {
            // ARRANGE
            var order = await CreateValidOrderOnDB();
            var request = new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Preparing };
            var itemId = order.Items.First().Id;

            // ACT
            var result = await _orderCommand.UpdateStatusItemOrder(order.Id, itemId, request);

            // ASSERT
            result.Should().NotBeNull();
            var updatedItem = result.Items.First(i => i.Id == itemId);
            updatedItem.StatusId.Should().Be((int)OrderItemStatus.Preparing);
        }


        [Fact]
        public async Task UpdateStatusItemOrder_AllItemsPreparing_UpdatesOverallStatusToPreparing()
        {
            // ARRANGE
            var order = await CreateOrderWithMultipleItems(3);
            var request = new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Preparing };

            // ACT
            foreach (var item in order.Items)
            {
                await _orderCommand.UpdateStatusItemOrder(order.Id, item.Id, request);
            }

            // ASSERT
            order.OverallStatusID.Should().Be((int)OrderItemStatus.Preparing);
        }

        [Fact]
        public async Task UpdateStatusItemOrder_AllItemsReady_UpdatesOverallStatusToReady()
        {
            // ARRANGE
            var order = await CreateOrderWithMultipleItems(2);
            foreach (var item in order.Items)
            {
                await _orderCommand.UpdateStatusItemOrder(order.Id, item.Id,
                    new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Preparing });
            }

            // ACT
            foreach (var item in order.Items)
            {
                await _orderCommand.UpdateStatusItemOrder(order.Id, item.Id,
                    new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Ready });
            }

            var result = await _context.Orders.Include(o => o.Items).FirstAsync(o => o.Id == order.Id);

            // ASSERT
            result.OverallStatusID.Should().Be((int)OrderItemStatus.Ready);
        }

        [Fact]
        public async Task UpdateStatusItemOrder_AllItemsDelivered_UpdatesOverallStatusToDelivered()
        {
            // ARRANGE
            var order = await CreateOrderWithMultipleItems(2);
            foreach (var item in order.Items)
            {
                await _orderCommand.UpdateStatusItemOrder(order.Id, item.Id,
                    new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Preparing });
                await _orderCommand.UpdateStatusItemOrder(order.Id, item.Id,
                    new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Ready });
                await _orderCommand.UpdateStatusItemOrder(order.Id, item.Id,
                    new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Delivered });
            }

            var result = await _context.Orders.Include(o => o.Items).FirstAsync(o => o.Id == order.Id);

            // ASSERT
            result.OverallStatusID.Should().Be((int)OrderItemStatus.Delivered);
        }

        [Fact]
        public async Task UpdateStatusItemOrder_InvalidOrderId_ThrowsInvalidOrderIdException()
        {
            // ARRANGE
            var nonExistentOrderId = 99999;
            var request = new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Preparing };

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOrderIdException>(
                () => _orderCommand.UpdateStatusItemOrder(nonExistentOrderId, 1, request));
        }

        [Fact]
        public async Task UpdateStatusItemOrder_InvalidItemId_ThrowsOrderItemNotFoundException()
        {
            // ARRANGE
            var order = await CreateValidOrderOnDB();
            var nonExistentItemId = 99999;
            var request = new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Preparing };

            // ACT & ASSERT
            await Assert.ThrowsAsync<OrderItemNotFoundException>(
                () => _orderCommand.UpdateStatusItemOrder(order.Id, nonExistentItemId, request));
        }

        [Fact]
        public async Task UpdateStatusItemOrder_ItemNotBelongsToOrder_ThrowsOrderItemNotFoundException()
        {
            // ARRANGE
            var order1 = await CreateValidOrderOnDB();
            var order2 = await CreateValidOrderOnDB();
            var itemFromOrder2 = order2.Items.First().Id;
            var request = new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Preparing };

            // ACT & ASSERT
            await Assert.ThrowsAsync<OrderItemNotFoundException>(
                () => _orderCommand.UpdateStatusItemOrder(order1.Id, itemFromOrder2, request));
        }

        [Fact]
        public async Task UpdateStatusItemOrder_InvalidStatusTransition_ThrowsInvalidOrderStatusTransitionException()
        {
            // ARRANGE
            var order = await CreateValidOrderOnDB();
            var itemId = order.Items.First().Id;

            var request = new OrderItemUpdateRequest { Status = (int)OrderItemStatus.Delivered };

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOrderStatusTransitionException>(
                () => _orderCommand.UpdateStatusItemOrder(order.Id, itemId, request));
        }
    }
}
