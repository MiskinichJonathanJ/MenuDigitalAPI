using Application.DataTransfers.Request.Order;
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

            mapper.Setup(m => m.ToEntity(It.IsAny<OrderRequest>()))
                  .Returns(new Order { Id = 1000, DeliveryTo = "calle test"});

            mapper.Setup(m => m.ToEntityItems(It.IsAny<ICollection<ItemRequest>>(), It.IsAny<int>()))
                  .Returns(new List<OrderItem>());

            mapper.Setup(m => m.ToResponse(It.IsAny<Order>(), It.IsAny<double>()))
                  .Returns((Order o, double total) => new OrderResponse
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
            Assert.Equal(18000.5, result.TotalMount, 1);
        }
    }
}
