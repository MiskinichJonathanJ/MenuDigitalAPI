using Application.DataTransfers.Request.Dish;
using Application.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Unit.UseCase.Dish
{
    public class DishService_CreateTests : DishServiceTestBase
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
            mockCommand.Setup(c => c.CreateDish(It.IsAny<Domain.Entities.Dish>())).Returns(Task.CompletedTask);

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
        public async Task CreateDish_WhenInvalidPrice_ReturnInvalidDishPriceException()
        {
            // ARRANGE
            var dishRequest = new DishRequest
            {
                Name = "test name",
                Description = "test description",
                Price = -1244,
                Category = 1,
                Image = "test URL"
            };

            mockValidator.Setup(v => v.ValidateCreate(It.IsAny<DishRequest>()))
                         .ThrowsAsync(new Application.Exceptions.InvalidDishPriceException("El precio del dish no puede ser negativo"));
            //Assert
            await Assert.ThrowsAsync<InvalidDishPriceException>(() => service.CreateDish(dishRequest));
            mockValidator.Verify(v => v.ValidateCreate(dishRequest), Times.Once);
            VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateDish_WhenNameEmpty_ReturnArgumentException()
        {
            // ARRANGE
            var dishRequest = new DishRequest
            {
                Name = "",
                Description = "test description",
                Price = 1244,
                Category = 1,
                Image = "test URL"
            };
            mockValidator.Setup(v => v.ValidateCreate(It.IsAny<DishRequest>()))
                         .ThrowsAsync(new ArgumentException("El nombre del dish no puede estar vacío"));
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateDish(dishRequest));
            mockValidator.Verify(v => v.ValidateCreate(dishRequest), Times.Once);
            VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateDish_WhenInvalidCategory_ReturnCategoryNotFoundException()
        {
            // ARRANGE
            var dishRequest = new DishRequest
            {
                Name = "test name",
                Description = "test description",
                Price = 1244,
                Category = 999, // Assuming this category does not exist
                Image = "test URL"
            };
            mockValidator.Setup(v => v.ValidateCreate(It.IsAny<DishRequest>()))
                         .ThrowsAsync(new CategoryNotFoundException("La categoria no existe"));

            //Assert
            await Assert.ThrowsAsync<CategoryNotFoundException>(() => service.CreateDish(dishRequest));
            mockValidator.Verify(v => v.ValidateCreate(dishRequest), Times.Once);
            VerifyNoOtherCalls();
        }
    }
}
