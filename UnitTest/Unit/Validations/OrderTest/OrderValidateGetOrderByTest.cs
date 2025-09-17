using Application.Exceptions.OrderException;
using Application.Validations;
using FluentAssertions;

namespace UnitTest.Unit.Validations.OrderTest
{
    public class OrderValidateGetOrderByTest
    {
        private readonly OrderValidator _validator  =  new();
        [Fact]
        public async Task ValidateGetOrderById_ValidId_DoesNotThrowException()
        {
            // Arrange
            int validOrderId = 1;
            // Act & Assert
            await FluentActions.Invoking(() => _validator.ValidateGetOrderById(validOrderId))
                .Should().NotThrowAsync();
        }
        [Fact]
        public async Task ValidateGetOrderById_InvalidId_ThrowsInvalidOrderIdException()
        {
            // Arrange
            int invalidOrderId = 0;
            // Act & Assert
            await FluentActions.Invoking(() => _validator.ValidateGetOrderById(invalidOrderId))
                .Should().ThrowAsync<InvalidOrderIdException>();
        }
    }
}
