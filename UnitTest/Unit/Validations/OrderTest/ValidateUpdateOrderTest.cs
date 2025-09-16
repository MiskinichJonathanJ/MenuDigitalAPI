using Application.DataTransfers.Request.Order;
using Application.Exceptions.OrderException;
using Application.Validations;
using FluentAssertions;

namespace UnitTest.Unit.Validations.OrderTest
{
    public class ValidateUpdateOrderTest
    {
        private readonly OrderValidator _validator = new();
        private static OrderUpdateRequest CreateValidOrderUpdateRequest() => new()
        {
            Items =
            [
                new() { Id = Guid.NewGuid(), Quantity = 2 },
            new() { Id = Guid.NewGuid(), Quantity = 1 }
            ]
        };
        [Fact]
        public async Task ValidateUpdateOrder_WithValidParams_Succeeds()
        {
            // ARRANGE
            var validRequest = CreateValidOrderUpdateRequest();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateOrder(validRequest))
                .Should().NotThrowAsync();
        }

        [Fact]
        public async Task ValidateUpdateOrder_WithEmptyItems_ThrowsEmptyOrderItemsException()
        {
            // ARRANGE
            var requestWithEmptyItems = new OrderUpdateRequest
            {
                Items = []
            };

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateOrder(requestWithEmptyItems))
                .Should().ThrowAsync<EmptyOrderItemsException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ValidateUpdateOrder_WithInvalidQuantity_ThrowsInvalidItemQuantityException(int invalidQuantity)
        {
            // ARRANGE
            var request = CreateValidOrderUpdateRequest();
            request.Items.First().Quantity = invalidQuantity;

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateOrder(request))
                .Should().ThrowAsync<InvalidItemQuantityException>();
        }
        [Fact]
        public async Task ValidateUpdateOrder_WithEmptyGuidId_ThrowsInvalidIdItemException()
        {
            // ARRANGE
            var request = CreateValidOrderUpdateRequest();
            request.Items.First().Id = Guid.Empty;

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateOrder(request))
                .Should().ThrowAsync<InvalidIdItemException>();
        }

        [Fact]
        public async Task ValidateUpdateOrder_WithMultipleItemsOneInvalidQuantity_ThrowsInvalidItemQuantityException()
        {
            // ARRANGE
            var request = new OrderUpdateRequest
            {
                Items =
                [
                    new() { Id = Guid.NewGuid(), Quantity = 2 },
                new() { Id = Guid.NewGuid(), Quantity = 0 },
                new() { Id = Guid.NewGuid(), Quantity = 3 }
                ]
            };

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateOrder(request))
                .Should().ThrowAsync<InvalidItemQuantityException>();
        }

        [Fact]
        public async Task ValidateUpdateOrder_WithMultipleItemsOneInvalidId_ThrowsInvalidIdItemException()
        {
            // ARRANGE
            var request = new OrderUpdateRequest
            {
                Items =
                [
                    new() { Id = Guid.NewGuid(), Quantity = 2 },
                new() { Id = Guid.Empty, Quantity = 1 },
                new() { Id = Guid.NewGuid(), Quantity = 3 }
                ]
            };

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateOrder(request))
                .Should().ThrowAsync<InvalidIdItemException>();
        }

    }
}
