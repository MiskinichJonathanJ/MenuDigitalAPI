using Application.DataTransfers.Request.Order;
using Application.Exceptions.OrderException;
using Application.Validations;
using FluentAssertions;

namespace UnitTest.Unit.Validations.OrderTest
{
    public class OrderValidateCreateOrderTest
    {
        private const int HOME_DELIVERY_TYPE = 1;
        private const int PICKUP_DELIVERY_TYPE = 2;
        private readonly OrderValidator _validator = new();
        private static OrderRequest CreateOrderRequestValid() => new()
        {
            Items =
                [
                    new() { Id = Guid.NewGuid(), Quantity = 2 }
                ],
            Delivery = new Delivery
            {
                Id = 1,
                To = "123 Main St"
            }
        };
        public static IEnumerable<object[]> InvalidItemsData =>
            [
                [null!],                    
                [new List<Items>()]     
            ];
        public static IEnumerable<object[]> ValidDeliveryData =>
            [
                [HOME_DELIVERY_TYPE, "123 Main St"],
                [PICKUP_DELIVERY_TYPE, null!],    
                [PICKUP_DELIVERY_TYPE, ""],          
                [3, null!],                
                [3, "Some address"]
            ];

        [Theory]
        [MemberData(nameof(ValidDeliveryData))]
        public async Task ValidateCreateOrder_WithValidDeliveryParams_Succeeds(int deliveryId, string? deliveryAddress)
        {
            // ARRANGE
            var request = CreateOrderRequestValid();
            request.Delivery.Id = deliveryId;
            request.Delivery.To = deliveryAddress;

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateCreateOrder(request))
                .Should().NotThrowAsync();
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

        [Theory]
        [MemberData(nameof(InvalidItemsData))]
        public async Task ValidateCreateOrder_WithInvalidItems_ThrowsEmptyOrderItemsException(ICollection<Items>? items)
        {
            // ARRANGE
            var request = CreateOrderRequestValid();
            request.Items = items!;
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
