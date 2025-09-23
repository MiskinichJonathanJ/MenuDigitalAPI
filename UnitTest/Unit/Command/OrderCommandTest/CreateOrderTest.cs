using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Unit.Command.OrderCommandTest
{
    public class CreateOrderTest : OrderCommandTestBase
    {
        [Fact]
        public async Task CreateOrder_WithValidOrder_ReturnsOrderWithGeneratedID()
        {
            // ARRANGE
            var order = CreateOrderValid();
            var countOrders = await _context.Order.CountAsync();

            //  ACT
            var result = await _orderCommand.CreateOrder(order);

            // ASSERT
            Assert.NotNull(result);
            result.OrderId.Should().Be(order.OrderId);
            result.Items.First().Order.Should().Be(result.OrderId);

            var countAfter = await _context.Order.CountAsync();
            countAfter.Should().Be(countOrders + 1);

            var savedOrder = await _context.Order.FindAsync(result.OrderId);
            savedOrder.Should().NotBeNull();
        }
    }
}
