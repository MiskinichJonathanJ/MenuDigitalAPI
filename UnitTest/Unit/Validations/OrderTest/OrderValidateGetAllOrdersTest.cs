using Application.Validations;
using FluentAssertions;
using Application.Exceptions.OrderException;
using Application.Exceptions.StatusException;
using Application.Validations.Helpers;

namespace UnitTest.Unit.Validations.OrderTest
{
    public class OrderValidateGetAllOrdersTest
    {
        private readonly OrderValidator _validator = new();
        private  const OrderItemStatusFlow.OrderItemStatus ValidStatus = OrderItemStatusFlow.OrderItemStatus.Preparing;
        [Fact]
        public async Task ValidateGetAllOrders_WithValidParams_Succeeds()
        {
            // ARRANGE
            DateTime? from = DateTime.Now.AddDays(-5);
            DateTime? to = DateTime.Now;
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateGetAllOrders(from, to, (int)ValidStatus)).Should().NotThrowAsync();
        }

        [Fact]
        public async Task ValidateGetAllOrders_WithInvalidFromDate_ThrowsInvalidDateOrderException()
        {
            // ARRANGE
            DateTime? from = DateTime.Now.AddDays(3);
            DateTime? to = DateTime.Now;
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateGetAllOrders(from, to, (int)ValidStatus)).Should().ThrowAsync<InvalidDateOrderException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(999)]
        [InlineData(int.MaxValue)] 
        [InlineData(int.MinValue)]  
        public async Task ValidateGetAllOrders_WithInvalidStatus_ThrowsStatusNotFoundException(int invalidStatus)
        {
            // ARRANGE
            DateTime? from = DateTime.Now.AddDays(-5);
            DateTime? to = DateTime.Now;

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateGetAllOrders(from, to, invalidStatus))
                .Should().ThrowAsync<StatusNotFoundException>();
        }
        [Fact]
        public async Task ValidateGetAllOrders_WithNullParams_Succeeds()
        {
            // ARRANGE
            DateTime? from = null;
            DateTime? to = null;
            int? status = null;
            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateGetAllOrders(from, to, status)).Should().NotThrowAsync();
        }

        [Fact]
        public async Task ValidateGetAllOrders_WithOnlyToDate_Succeeds()
        {
            // ARRANGE
            DateTime? from = null;
            DateTime? to = DateTime.Now;
            int? status = 2;

            // ACT & ASSERT
            await FluentActions.Invoking(() => _validator.ValidateGetAllOrders(from, to, status))
                .Should().NotThrowAsync();
        }
    }
}
