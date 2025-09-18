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
    }
}
