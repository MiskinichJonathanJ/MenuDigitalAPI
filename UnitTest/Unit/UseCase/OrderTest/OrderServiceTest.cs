using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.DataTransfers.Response.Dish;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderItem;
using Application.DataTransfers.Response.OrderResponse;
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
                    new Items { Id = Guid.NewGuid(), Quantity = 1, Notes = "Sin queso" },
            new Items { Id = Guid.NewGuid(), Quantity = 2, Notes = "Con todo" }
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

            mapper.Setup(m => m.ToEntityItems(It.IsAny<ICollection<Items>>(), It.IsAny<int>()))
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
            mapper.Setup(m => m.ToDetailsResponse(It.IsAny<Order>()))
                  .Returns((Order o) => new OrderDetailsResponse
                  {
                    OrderNumber = o.Id,
                    CreatedDate = o.CreateDate,
                    Items = new List<OrderItemResponse>()
                  });

            // ACT
            var result = await service.GetAllOrders(desde, hasta, statusOrders);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.First().OrderNumber);
            Assert.Equal(2, result.Last().OrderNumber);
        }

        [Fact]
        public async Task GetOrderById_ValidParams_ReturnsOrderDetailsResponse()
        {
            int orderId = 1;
            Guid dishId = Guid.NewGuid();
            Guid dishId2 = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                DeliveryTo = "Calle 123",
                CreateDate = DateTime.Now,
                Items = new List<OrderItem>
                {
                    new() { Id = 1, Quantity = 1, Notes = "Sin queso",OrderId = orderId, StatusId =  1, DishId = dishId, DishNav = new Domain.Entities.Dish { Name = "Pizza", Description = "test", ImageURL = "URLtest" } },
                    new() { Id = 2, Quantity = 2, Notes = "Con todo", OrderId = orderId, StatusId =  1, DishId = dishId2, DishNav = new Domain.Entities.Dish { Name = "Pasta", Description = "test", ImageURL = "URLtest" } }
                }
            };

            validator.Setup(v => v.ValidateGetOrderById(orderId)).Returns(Task.CompletedTask);
            query.Setup(q => q.GetOrderById(orderId)).ReturnsAsync(order);
            mapper.Setup(m => m.ToDetailsResponse(It.IsAny<Order>()))
                  .Returns((Order o) => new OrderDetailsResponse
                  {
                      OrderNumber = o.Id,
                      CreatedDate = o.CreateDate,
                      Items = o.Items.Select(i => new OrderItemResponse
                      {
                          Id = i.Id,
                          Quantity = i.Quantity,
                          Notes = i.Notes,
                          Dish = new DishShortResponse
                          {
                              Id = i.DishId,
                              Name = i.DishNav?.Name ?? "Unknown",
                              Image = i.DishNav?.ImageURL ?? "No image"
                          }
                      }).ToList()
                  });
            // ACT
            var result = await service.GetOrderById(orderId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderNumber);
            Assert.Equal(2, result.Items?.Count);
            Assert.Equal(dishId, result.Items?.First().Dish?.Id ?? Guid.Empty);
            Assert.Equal(dishId2, result.Items?.Last().Dish?.Id ?? Guid.Empty);
        }

        [Fact]
        public async Task GetOrderById_InvalidId_ThrowsException()
        {
            // ARRANGE
            int invalidOrderId = -1;
            validator.Setup(v => v.ValidateGetOrderById(invalidOrderId))
                     .ThrowsAsync(new Exception("El ID de la orden no es válido."));
            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<Exception>(() => service.GetOrderById(invalidOrderId));
            Assert.Equal("El ID de la orden no es válido.", exception.Message);
        }

        [Fact]
        public async  Task UpdateStatusItemOrder_ValidParam_ReturnsOrderUpdateResponse()
        {
            var orderId = 1;
            var itemId = 1;
            OrderItemUpdateRequest request = new()
            {
                Status = 2
            };
            var orderItem = new OrderItem
            {
                Id = itemId,
                OrderId = orderId,
                StatusId = 1,
                Quantity = 1,
                Notes = "Sin queso",
                DishId = Guid.NewGuid(),
                CreatedDate = DateTime.Now
            };

            validator.Setup(v => v.ValidateUpdateStatusItemOrder(orderId, itemId, request)).Returns(Task.CompletedTask);
            command.Setup(c => c.UpdateStatusItemOrder(itemId, orderId, request)).ReturnsAsync(new Order { DeliveryTo = "test"});
            mapper.Setup(m => m.ToUpdateResponse(It.IsAny<Order>()))
                .Returns((Order o) => new OrderUpdateResponse
                {
                    OrderNumber = orderId,
                    UpdatedDate = DateTime.Now
                });

            // ACT
            var result = await service.UpdateStatusItemOrder(orderId, itemId, request);
            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderNumber);
        }
    }
}
