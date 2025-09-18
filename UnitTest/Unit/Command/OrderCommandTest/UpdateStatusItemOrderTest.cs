using Application.DataTransfers.Request.OrderItem;
using Application.Exceptions.OrderException;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using static Application.Validations.Helpers.OrderItemStatusFlow;

namespace UnitTest.Unit.Command.OrderCommandTest
{
    public class UpdateStatusItemOrderTest : OrderCommandTestBase
    {
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
            await Assert.ThrowsAsync<OrderNotFoundException>(
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
