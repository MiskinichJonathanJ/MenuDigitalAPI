using Application.DataTransfers.Request.OrderItem;
using Application.Exceptions.OrderException;
using Application.Exceptions.StatusException;
using Application.Validations;
using Application.Validations.Helpers;
using FluentAssertions;

namespace UnitTest.Unit.Validations.OrderTest
{
    public class ValidateUpdateStatusItemOrderTest
    {
        private readonly OrderValidator _validator = new();
        private readonly OrderItemUpdateRequest _validRequest = new()
        {
            Status = (int)OrderItemStatusFlow.OrderItemStatus.Preparing
        };
        private readonly OrderItemUpdateRequest _invalidStatusRequest = new()
        {
            Status = -999
        };

        [Fact]
        public async Task ValidateUpdateStatusItemOrder_InvalidStatus_ThrowsStatusNotFoundException()
        {
            // ARRANGE
            var orderId = 1;
            var itemId = 1;
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateStatusItemOrder(orderId, itemId, _invalidStatusRequest)).Should().ThrowAsync<StatusNotFoundException>();
        }

        [Theory]
        [InlineData(0, 1)]    
        [InlineData(-1, 1)]    
        [InlineData(1, 0)]    
        [InlineData(1, -1)]    
        [InlineData(0, 0)]    
        [InlineData(-5, -10)]  
        public async Task ValidateUpdateStatusItemOrder_WithInvalidIds_ThrowsInvalidOrderIdException(int orderId, int itemId)
        {
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateStatusItemOrder(orderId, itemId, _validRequest))
                .Should().ThrowAsync<InvalidOrderIdException>();
        }
    }
}
