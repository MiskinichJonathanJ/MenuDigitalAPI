using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response.Dish;
using Application.Exceptions;
using Application.Exceptions.DishException;
using FluentAssertions;
using Moq;

namespace UnitTest.Unit.UseCase.Dish
{

    public class DishServiceTest : DishServiceTestBase
    {
        [Fact]
        public async Task CreateDish_WhenValidRequest_ReturnDishResponse()
        {
            // ARRANGE
            var validRequest = BuildValidRequest();
            var dishEntity = BuildEntity(validRequest);
            var expectedResponse = BuildResponse(dishEntity);

            mockMapper.Setup(m => m.ToEntity(It.IsAny<DishRequest>())).Returns(dishEntity);
            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>())).Returns(expectedResponse);
            mockValidator.Setup(v => v.ValidateCreate(It.IsAny<DishRequest>())).Returns(Task.CompletedTask);
            mockCommand.Setup(c => c.CreateDish(It.IsAny<Domain.Entities.Dish>())).ReturnsAsync(dishEntity);

            // ACT
            var result = await service.CreateDish(validRequest);

            // ASSERT
            mockMapper.Verify(m => m.ToEntity(validRequest), Times.Once);
            mockMapper.Verify(m => m.ToResponse(dishEntity), Times.Once);
            mockValidator.Verify(v => v.ValidateCreate(validRequest), Times.Once);
            mockCommand.Verify(c => c.CreateDish(dishEntity), Times.Once);
            VerifyNoOtherCalls();

            result.name.Should().Be(validRequest.Name);
            result.Description.Should().Be(validRequest.Description);
            result.Price.Should().Be(validRequest.Price);
            result.Image.Should().Be(validRequest.Image);
            result.category.Should().NotBeNull();
            result.category.id.Should().Be(validRequest.Category);
        }

        [Fact]
        public async Task UpdateDish_ValidRequest_ReturnDishResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            var validRequest = BuildValidRequest();
            var dishEntity = BuildEntity(validRequest);
            var dishRequest = new DishUpdateRequest
            {
                Name = validRequest.Name,
                Description = validRequest.Description,
                Price = validRequest.Price,
                Category = validRequest.Category,
                Image = validRequest.Image,
                IsActive = true
            };
            var expectedResponse = BuildResponse(dishEntity);
            expectedResponse.IsActive = true;

            mockValidator.Setup(v => v.ValidateUpdate(It.IsAny<Guid>(), It.IsAny<DishUpdateRequest>())).Returns(Task.CompletedTask);
            mockCommand.Setup(c => c.UpdateDish(It.IsAny<Guid>(), It.IsAny<DishUpdateRequest>())).ReturnsAsync(dishEntity);
            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>())).Returns(expectedResponse);

            // ACT
            var result = await service.UpdateDish(dishId, dishRequest);

            // ASSERT
            mockValidator.Verify(v => v.ValidateUpdate(dishId, dishRequest), Times.Once);
            mockCommand.Verify(c => c.UpdateDish(dishId, dishRequest), Times.Once);
            result.name.Should().Be(expectedResponse.name);
            result.Description.Should().Be(expectedResponse.Description);
            result.Price.Should().Be(expectedResponse.Price);
            result.Image.Should().Be(expectedResponse.Image);
            result.category.Should().NotBeNull();
            result.category.id.Should().Be(expectedResponse.category?.id ?? 1);
            result.ID.Should().Be(expectedResponse.ID);
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateDish_InvalidIdRequest_ReturnDishResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            var validRequest = BuildValidRequest();
            var dishRequest = new DishUpdateRequest
            {
                Name = validRequest.Name,
                Description = validRequest.Description,
                Price = validRequest.Price,
                Category = validRequest.Category,
                Image = validRequest.Image,
                IsActive = true
            };

            mockValidator.Setup(v => v.ValidateUpdate(It.IsAny<Guid>(), It.IsAny<DishUpdateRequest>())).Returns(Task.CompletedTask);
            mockCommand.Setup(c => c.UpdateDish(It.IsAny<Guid>(), It.IsAny<DishUpdateRequest>())).ThrowsAsync(new DishNotFoundException());

            //Assert
            await Assert.ThrowsAsync<DishNotFoundException>(() => service.UpdateDish(dishId, dishRequest));
        }

        public static TheoryData<string?, int?, bool?, string?> ValidSearchParameters => new()
            {
                { "Pizza", null, null, null },
                { null, 1, null, null },
                { null, null, true, null },
                { null, null, null, "asc" },
                { "Pasta", 2, false, "desc" }
            };


        [Theory]
        [MemberData(nameof(ValidSearchParameters))]
        public async Task GetAllDish_WithVariousValidParams_ReturnsResults(string name, int? categoryId, bool? onlyActive, string? sortByPrice)
        {
            // ARRANGE
            var expectedDishes = new List<Domain.Entities.Dish> {};
            mockQuery.Setup(q => q.GetAllDish(name, categoryId, onlyActive, sortByPrice))
                     .ReturnsAsync(expectedDishes);

            // ACT
            var result = await service.GetAllDish(name, categoryId, onlyActive, sortByPrice);

            // ASSERT
            result.Should().NotBeNull();
            mockQuery.Verify(q => q.GetAllDish(name, categoryId, onlyActive, sortByPrice), Times.Once);
        }

        [Fact]
        public async Task GetDishById_WhenDishExists_ReturnsMappedResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            var validRequest = BuildValidRequest();
            var dishEntity = BuildEntity(validRequest);
            dishEntity.DishId = dishId;
            var expectedResponse = BuildResponse(dishEntity);
            expectedResponse.ID = dishId;

            mockQuery.Setup(q => q.GetDishById(dishId)).ReturnsAsync(dishEntity);
            mockMapper.Setup(m => m.ToResponse(dishEntity)).Returns(expectedResponse);

            // ACT
            var result = await service.GetDishById(dishId);

            // ASSERT
            mockQuery.Verify(q => q.GetDishById(dishId), Times.Once);
            mockMapper.Verify(m => m.ToResponse(dishEntity), Times.Once);
            VerifyNoOtherCalls();

            result.Should().NotBeNull();
            result.ID.Should().Be(expectedResponse.ID);
            result.name.Should().Be(expectedResponse.name);
            result.Description.Should().Be(expectedResponse.Description);
            result.Image.Should().Be(expectedResponse.Image);
            result.category?.id.Should().Be(expectedResponse.category?.id ?? 1);
        }

        [Fact]
        public async Task GetDishById_WhenDishDoesNotExist_ThrowsDishNotFoundException()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            Domain.Entities.Dish? dishEntity = null;
           
            mockQuery.Setup(q => q.GetDishById(dishId)).ReturnsAsync(dishEntity);
            
            // ACT & ASSERT
            await Assert.ThrowsAsync<DishNotFoundException>(() => service.GetDishById(dishId));
            mockQuery.Verify(q => q.GetDishById(dishId), Times.Once);
            VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteDish_WhenDishExists_ReturnsMappedResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            var validRequest = BuildValidRequest();
            var dishEntity = BuildEntity(validRequest);
            dishEntity.DishId = dishId;

            mockQuery.Setup(q => q.GetDishById(dishId)).ReturnsAsync(dishEntity);
            mockCommand.Setup(c => c.DeleteDish(dishId)).Returns(Task.CompletedTask);
            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>())).Returns(BuildResponse(dishEntity));

            // ACT
            await service.DeleteDish(dishId);

            // ASSERT
            mockQuery.Verify(q => q.GetDishById(dishId), Times.Once);
            mockCommand.Verify(c => c.DeleteDish(dishId), Times.Once);
            mockMapper.Verify(m => m.ToResponse(dishEntity), Times.Once);
            VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteDish_WhenDishDoesNotExists_ReturnsMappedResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            Domain.Entities.Dish? dishEntity = null;

            mockQuery.Setup(q => q.GetDishById(dishId)).ReturnsAsync(dishEntity);

            // ACT & ASSERT
            await Assert.ThrowsAsync<DishNotFoundException>(() => service.DeleteDish(dishId));
            mockQuery.Verify(q => q.GetDishById(dishId), Times.Once);
            VerifyNoOtherCalls();
        }
    }
}
