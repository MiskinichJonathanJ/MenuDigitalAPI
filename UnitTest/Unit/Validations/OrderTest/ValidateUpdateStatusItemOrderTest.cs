using Application.DataTransfers.Request.OrderItem;
using Application.Exceptions.OrderException;
using Application.Exceptions.StatusException;
using Application.Validations;
using Application.Validations.Helpers;
using Domain.Entities;
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
        private static OrderItemUpdateRequest CreateValidRequest()
        {
            return new() { Status = 1 };
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 10)]
        [InlineData(100, 999)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task ValidateUpdateStatusItemOrder_WithValidIds_Succeeds(int orderId, int itemId)
        {
            // ARRANGE
            var validRequest = CreateValidRequest();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateStatusItemOrder(orderId, itemId, validRequest))
                .Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)] 
        [InlineData(3)] 
        [InlineData(4)] 
        public async Task ValidateUpdateStatusItemOrder_WithValidStatus_Succeeds(int validStatus)
        {
            // ARRANGE 
            var request = new OrderItemUpdateRequest { Status = validStatus };

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateUpdateStatusItemOrder(1, 1, request))
                .Should().NotThrowAsync();
        }

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
