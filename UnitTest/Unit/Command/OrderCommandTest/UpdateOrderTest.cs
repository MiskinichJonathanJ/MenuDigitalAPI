using Application.Exceptions.OrderException;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using static Application.Validations.Helpers.OrderItemStatusFlow;

namespace UnitTest.Unit.Command.OrderCommandTest
{
    public class UpdateOrderTest : OrderCommandTestBase
    {
        [Fact]
        public async Task UpdateOrder_ModifyExistingItems_UpdatesItemsSuccessfully()
        {
            // ARRANGE
            var order = await CreateOrderWithMultipleItems(4);
            var existingDishId = order.Items.First().Dish;
            var newPrice = 150.75m;

            var updateRequest = new List<OrderItem>
            {
                new OrderItem
                {
                    Order = order.OrderId,
                    Dish = existingDishId,
                    Quantity = 5, 
                    Notes = "Notas actualizadas", 
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)OrderItemStatus.Pending
                }
            };

            // ACT
            var result = await _orderCommand.UpdateOrder(updateRequest, order.OrderId, newPrice);

            // ASSERT
            result.Should().NotBeNull();
            result.Price.Should().Be(newPrice);
            result.UpdateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            var updatedItem = result.Items.First(i => i.Dish == existingDishId);
            updatedItem.Quantity.Should().Be(5);
            updatedItem.Notes.Should().Be("Notas actualizadas");

            var savedOrder = await _context.Order.Include(o => o.Items)
                .FirstAsync(o => o.OrderId == order.OrderId);
            savedOrder.Items.First(i => i.Dish == existingDishId).Quantity.Should().Be(5);
        }
        [Fact]
        public async Task UpdateOrder_AddNewItems_AddsItemsSuccessfully()
        {
            // ARRANGE
            var order = await CreateOrderWithMultipleItems(3);
            var originalItemCount = order.Items.Count;
            var newDishId = Guid.NewGuid();
            var newPrice = 200.50m;

            var updateRequest = new List<OrderItem>
            {
                new OrderItem
                {
                    Order = order.OrderId,
                    Dish = newDishId,
                    Quantity = 2,
                    Notes = "Plato nuevo",
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)OrderItemStatus.Pending
                }
            };

            // ACT
            var result = await _orderCommand.UpdateOrder(updateRequest, order.OrderId, newPrice);

            // ASSERT
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(originalItemCount + 1);
            result.Price.Should().Be(newPrice);

            var newItem = result.Items.First(i => i.Dish == newDishId);
            newItem.Quantity.Should().Be(2);
            newItem.Notes.Should().Be("Plato nuevo");
            newItem.Order.Should().Be(order.OrderId);
        }

        [Fact]
        public async Task UpdateOrder_MixAddAndModify_HandlesCorrectly()
        {
            // ARRANGE
            var order = await CreateOrderWithMultipleItems(2);
            var existingDishId = order.Items.First().Dish;
            var newDishId = Guid.NewGuid();
            var newPrice = 300.25m;

            var updateRequest = new List<OrderItem>
            {
                new OrderItem
                {
                    Order = order.OrderId,
                    Dish = existingDishId,
                    Quantity = 10,
                    Notes = "Cantidad modificada",
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)OrderItemStatus.Pending
                },

                new OrderItem
                {
                    Order = order.OrderId,
                    Dish = newDishId,
                    Quantity = 3,
                    Notes = "Item completamente nuevo",
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)OrderItemStatus.Pending
                }
            };

            // ACT
            var result = await _orderCommand.UpdateOrder(updateRequest, order.OrderId, newPrice);

            // ASSERT
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3); 
            result.Price.Should().Be(newPrice);

            var modifiedItem = result.Items.First(i => i.Dish == existingDishId);
            modifiedItem.Quantity.Should().Be(10);
            modifiedItem.Notes.Should().Be("Cantidad modificada");

            var newItem = result.Items.First(i => i.Dish == newDishId);
            newItem.Quantity.Should().Be(3);
            newItem.Notes.Should().Be("Item completamente nuevo");
        }
        [Fact]
        public async Task UpdateOrder_OrderClosed_ThrowsOrderNotFoundException()
        {
            // ARRANGE
            var order = await CreateOrderWithMultipleItems(2);
            order.OverallStatus = (int)OrderItemStatus.Closed;
            await _context.SaveChangesAsync();
            var newPrice = 300.25m;
            var updateRequest = new List<OrderItem>
            {
                new() {
                    Order = order.OrderId,
                    Dish = Guid.NewGuid(),
                    Quantity = 10,
                    Notes = "Cantidad modificada",
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)OrderItemStatus.Pending
                }
            };

            // ACT  & ASSERT
            await Assert.ThrowsAsync<OrderNotFoundException>(() => _orderCommand.UpdateOrder(updateRequest, order.OrderId, newPrice));
        }
    }
}
