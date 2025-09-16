using Application.DataTransfers.Request.Order;
using Application.Exceptions.OrderException;
using Application.Validations;
using FluentAssertions;

namespace UnitTest.Unit.Validations.OrderTest
{
    public class OrderValidateCreateOrderTest
    {
        private readonly OrderValidator _validator = new();
        private static OrderRequest CreateOrderRequestValid() => new()
        {
            Items =
                [
                    new() { Id = Guid.NewGuid(), Quantity = 2 }
                ],
            Delivery = new DeliveryRequest
            {
                Id = 1,
                To = "123 Main St"
            }
        };
        [Fact]
        public async Task ValidateCreateOrder_WithValidParams_Sucess()
        {
            // ARRANGE
            var request = CreateOrderRequestValid();
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateCreateOrder(request)).Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ValidateCreateOrder_InvalidQuantity_ThrowsInvalidItemQuantityException(int invalidQuantity)
        {
            // ARRANGE
            var request = CreateOrderRequestValid();
            request.Items.First().Quantity = invalidQuantity;

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateCreateOrder(request))
                .Should().ThrowAsync<InvalidItemQuantityException>();
        }
        [Fact]
        public async Task ValidateCreateOrder_WithInvalidDishId_ThrowsInvalidIdItemException()
        {
            // ARRANGE
            var request = CreateOrderRequestValid();
            request.Items.First().Id = Guid.Empty;
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateCreateOrder(request)).Should().ThrowAsync<InvalidIdItemException>();
        }

        [Fact]
        public async Task ValidateCreateOrder_WithInvalidItems_ThrowsEmptyOrderItemsException()
        {
            // ARRANGE
            var request = CreateOrderRequestValid();
            request.Items = [];
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateCreateOrder(request)).Should().ThrowAsync<EmptyOrderItemsException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ValidateCreateOrder_InvalidDeliveryAddress_ThrowsMissingAddressDeliveryException(string invalidAddress)
        {
            // ARRANGE
            var request = CreateOrderRequestValid();
            request.Delivery.To = invalidAddress;

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateCreateOrder(request))
                .Should().ThrowAsync<MissingAdrresDeliveryException>();

        }
    }
}
