using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderResponse;
using Application.Interfaces.IOrder;
using Application.UseCase.OrderUse;
using Domain.Entities;
using Moq;

namespace UnitTest.Unit.UseCase.OrderTest
{
    public class OrderServiceTest : OrderServiceTestBase
    {
        [Fact]
        public async Task CreateOrder_ValidParams_ReturnsOrderResponse()
        {
            // ARRANGE
            var orderCreate = new OrderRequest
            {
                Items = [
                    new ItemRequest { Id = Guid.NewGuid(), Quantity = 1, Notes = "Sin queso" },
            new ItemRequest { Id = Guid.NewGuid(), Quantity = 2, Notes = "Con todo" }
                ],
                Delivery = new DeliveryRequest { Id = 1, To = "Calle 123" },
                Notes = "Por favor, llegar antes de las 8 PM"
            };

            query.Setup(q => q.GetAllDishesOrder(orderCreate.Items)).ReturnsAsync(new List<Domain.Entities.Dish>
            {
                new() { ID = orderCreate.Items.First().Id, Price = 8000, Description = "test", ImageURL = "test", Name = "test"},
                new() {ID = orderCreate.Items.Last().Id, Price = 10000.5M, Description = "test", ImageURL= "test", Name = "test"}
            });

            double total = 8000 * 1 + 10000.5 * 2;

            mapper.Setup(m => m.ToEntity(It.IsAny<OrderRequest>()))
                  .Returns(new Order { Id = 1000, DeliveryTo = "calle test", Price = (decimal)total });

            mapper.Setup(m => m.ToEntityItems(It.IsAny<ICollection<ItemRequest>>(), It.IsAny<int>()))
                  .Returns(new List<OrderItem>());

            mapper.Setup(m => m.ToCreateResponse(It.IsAny<Order>()))
                  .Returns((Order o) => new OrderCreateResponse
                  {
                      OrderNumber = o.Id,
                      TotalMount = total,
                      CreatedDate = DateTime.Now
                  });

            // ACT
            var result = await service.CreateOrder(orderCreate);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(1000, result.OrderNumber);
            Assert.Equal(28001, result.TotalMount, 1);
        }

        [Fact]
        public async Task GetAllOrders_ValidParams_ReturnsListOfOrderResponse()
        {
            // ARRANGE
            var orders = new List<Order>
            {
                new() { Id = 1, DeliveryTo = "Calle 123", CreateDate = DateTime.Now },
                new() { Id = 2, DeliveryTo = "Avenida 456", CreateDate = DateTime.Now }
            };
            DateTime desde = DateTime.Now.AddDays(-10);
            DateTime hasta = DateTime.Now;
            int statusOrders = 1;

            validator.Setup(v => v.ValidateGetAllOrders(desde, hasta, statusOrders)).Returns(Task.CompletedTask);
            query.Setup(q => q.GetAllOrders(desde, hasta, statusOrders)).ReturnsAsync(orders);
            mapper.Setup(m => m.ToDetailsResponse(It.IsAny<ICollection<Order>>()))
                  .Returns((ICollection<Order> orders) => [.. orders
                      .Select(o => new OrderDetailsResponse
                      {
                          OrderNumber = o.Id,
                          CreatedDate = o.CreateDate
                      })]);

            // ACT
            var result = await service.GetAllOrders(desde, hasta, statusOrders);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.First().OrderNumber);
            Assert.Equal(2, result.Last().OrderNumber);
        }
    }
}
