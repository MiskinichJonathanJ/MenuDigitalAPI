using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.DataTransfers.Response.Dish;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderItem;
using Application.DataTransfers.Response.OrderResponse;
using Application.Exceptions.OrderException;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace UnitTest.Unit.UseCase.OrderTest
{
    public class OrderServiceTest : OrderServiceTestBase
    {
        [Fact]
        public async Task CreateOrder_WithNonExistentDish_ThrowsDishNotAvailableException()
        {
            // ARRANGE
            var nonExistentDishId = Guid.NewGuid();
            var existingDishId = Guid.NewGuid();

            var orderRequest = new OrderRequest
            {
                Items =
            [
                new() { Id = existingDishId, Quantity = 2 },      
                new() { Id = nonExistentDishId, Quantity = 1 },   
                new() { Id = Guid.NewGuid(), Quantity = 3 }       
            ],
                Delivery = new Delivery { Id = 1, To = "123 Main St" }
            };

            validator.Setup(v => v.ValidateCreateOrder(It.IsAny<OrderRequest>()))
                       .Returns(Task.CompletedTask);

            // Mock query - solo devuelve UN plato (falta uno de los solicitados)
            List<Domain.Entities.Dish> availableDishes = new List<Domain.Entities.Dish>
            {
                new() { DishId = existingDishId, Name = "Pizza", Price = 15.50m, Description = "test", ImageUrl = "url" }
            };

            query.Setup(q => q.GetAllDishesOrder(It.IsAny<ICollection<Items>>()))
                    .ReturnsAsync(availableDishes);

            // ACT & ASSERT
            await FluentActions.Invoking(() => service.CreateOrder(orderRequest))
                .Should().ThrowAsync<DishNotAvailableException>();

            validator.Verify(v => v.ValidateCreateOrder(orderRequest), Times.Once);
            query.Verify(q => q.GetAllDishesOrder(orderRequest.Items), Times.Once);
            command.Verify(c => c.CreateOrder(It.IsAny<Order>()), Times.Never);
            mapper.Verify(m => m.ToCreateResponse(It.IsAny<Order>()), Times.Never);
        }
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
                Delivery = new Delivery { Id = 1, To = "Calle 123" },
                Notes = "Por favor, llegar antes de las 8 PM"
            };

            query.Setup(q => q.GetAllDishesOrder(orderCreate.Items)).ReturnsAsync(new List<Domain.Entities.Dish>
            {
                new() { DishId = orderCreate.Items.First().Id, Price = 8000, Description = "test", ImageUrl = "test", Name = "test"},
                new() {DishId = orderCreate.Items.Last().Id, Price = 10000.5M, Description = "test", ImageUrl= "test", Name = "test"}
            });

            double total = 8000 * 1 + 10000.5 * 2;

            mapper.Setup(m => m.ToEntity(It.IsAny<OrderRequest>()))
                  .Returns(new Order { OrderId = 1000, DeliveryTo = "calle test", Price = (decimal)total });

            mapper.Setup(m => m.ToEntityItems(It.IsAny<ICollection<Items>>()))
                  .Returns(new List<OrderItem>());

            command.Setup(c => c.CreateOrder(It.IsAny<Order>())).ReturnsAsync(new Order { DeliveryTo  = "Calle 123", OrderId  = 1000});

            mapper.Setup(m => m.ToCreateResponse(It.IsAny<Order>()))
                  .Returns((Order o) => new OrderCreateResponse
                  {
                      OrderNumber = o.OrderId,
                      TotalAmount = total,
                      CreatedAt = DateTime.Now
                  });

            // ACT
            var result = await service.CreateOrder(orderCreate);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(1000, result.OrderNumber);
            Assert.Equal(28001, result.TotalAmount, 1);
        }

        [Fact]
        public async Task GetAllOrders_ValidParams_ReturnsListOfOrderResponse()
        {
            // ARRANGE
            var orders = new List<Order>
            {
                new() { OrderId = 1, DeliveryTo = "Calle 123", CreateDate = DateTime.Now },
                new() { OrderId = 2, DeliveryTo = "Avenida 456", CreateDate = DateTime.Now }
            };
            DateTime desde = DateTime.Now.AddDays(-10);
            DateTime hasta = DateTime.Now;
            int statusOrders = 1;

            validator.Setup(v => v.ValidateGetAllOrders(desde, hasta, statusOrders)).Returns(Task.CompletedTask);
            query.Setup(q => q.GetAllOrders(desde, hasta, statusOrders)).ReturnsAsync(orders);
            mapper.Setup(m => m.ToDetailsResponse(It.IsAny<Order>()))
                  .Returns((Order o) => new OrderDetailsResponse
                  {
                    OrderNumber = o.OrderId,
                    CreatedAt = o.CreateDate,
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
                OrderId = orderId,
                DeliveryTo = "Calle 123",
                CreateDate = DateTime.Now,
                Items = new List<OrderItem>
                {
                    new() { OrderItemId = 1, Quantity = 1, Notes = "Sin queso",Order = orderId, Status =  1, Dish = dishId, DishNav = new Domain.Entities.Dish { Name = "Pizza", Description = "test", ImageUrl = "URLtest" } },
                    new() { OrderItemId = 2, Quantity = 2, Notes = "Con todo", Order = orderId, Status =  1, Dish = dishId2, DishNav = new Domain.Entities.Dish { Name = "Pasta", Description = "test", ImageUrl = "URLtest" } }
                }
            };

            validator.Setup(v => v.ValidateGetOrderById(orderId)).Returns(Task.CompletedTask);
            query.Setup(q => q.GetOrderById(orderId)).ReturnsAsync(order);
            mapper.Setup(m => m.ToDetailsResponse(It.IsAny<Order>()))
                  .Returns((Order o) => new OrderDetailsResponse
                  {
                      OrderNumber = o.OrderId,
                      CreatedAt = o.CreateDate,
                      Items = o.Items.Select(i => new OrderItemResponse
                      {
                          Id = i.OrderItemId,
                          Quantity = i.Quantity,
                          Notes = i.Notes,
                          Dish = new DishShortResponse
                          {
                              Id = i.Dish,
                              Name = i.DishNav?.Name ?? "Unknown",
                              Image = i.DishNav?.ImageUrl ?? "No image"
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
                OrderItemId = itemId,
                Order = orderId,
                Status = 1,
                Quantity = 1,
                Notes = "Sin queso",
                Dish = Guid.NewGuid(),
                CreatedDate = DateTime.Now
            };

            validator.Setup(v => v.ValidateUpdateStatusItemOrder(orderId, itemId, request)).Returns(Task.CompletedTask);
            command.Setup(c => c.UpdateStatusItemOrder(itemId, orderId, request)).ReturnsAsync(new Order { DeliveryTo = "test"});
            mapper.Setup(m => m.ToUpdateResponse(It.IsAny<Order>()))
                .Returns((Order o) => new OrderUpdateResponse
                {
                    OrderNumber = orderId,
                    UpdateAt = DateTime.Now
                });

            // ACT
            var result = await service.UpdateStatusItemOrder(orderId, itemId, request);
            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderNumber);
        }
    }
}
