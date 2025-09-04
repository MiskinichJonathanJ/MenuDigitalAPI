using Application.DataTransfers.Request;
using Application.DataTransfers.Response;
using Application.Exceptions;
using Application.Interfaces.DishInterfaces;
using Application.UseCase.DishUse;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Unit.UseCase.Dish
{

    public class DishServiceTest
    {
        [Fact]
        public async Task CreateDish_ValidDish_ReturnDishResponse()
        {
            // ARRANGE
            var mockCommand = new Mock<IDishCommand>();
            var mockQuery = new Mock<IDishQuery>();
            var mockMapper = new Mock<IDishMapper>();

            var mockValidator = new Mock<IDishValidator>();
            var service = new DishServices(
                mockCommand.Object,
                mockQuery.Object,
                mockMapper.Object,
                mockValidator.Object
            );

            var dishRequest = new DishRequest
            {
                Name = "test name",
                Description = "test description",
                Price = 1244,
                Category = 1,
                Image = "test URL"
            };
            var dishEntity = new Domain.Entities.Dish
            {
                Name = dishRequest.Name,
                Description = dishRequest.Description,
                Price = dishRequest.Price,
                CategoryId = dishRequest.Category,
                ImageURL = dishRequest.Image
            };

            var expectedResponse = new DishResponse
            {
                name = dishRequest.Name,
                Description = dishRequest.Description,
                Price = dishRequest.Price,
                Image = dishRequest.Image,
                category = new GenericResponse { id = dishRequest.Category }
            };

            mockMapper.Setup(m => m.ToEntity(It.IsAny<DishRequest>())).Returns(dishEntity);
            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>())).Returns(expectedResponse);



            // ACT
            var result = await service.CreateDish(dishRequest);

            // ASSERT
            mockValidator.Verify(v => v.ValidateCreate(dishRequest), Times.Once);
            mockCommand.Verify(c => c.CreateDish(dishEntity), Times.Once);
            result.name.Should().Be(dishRequest.Name);
            result.Description.Should().Be(dishRequest.Description);
            result.Price.Should().Be(dishRequest.Price);
            result.Image.Should().Be(dishRequest.Image);
            result.category.id.Should().Be(dishRequest.Category);
        }

        [Fact]
        public async Task GetAllDish_ValidParameters_ReturnDishResponseList()
        {
            // ARRANGE
            var mockCommand = new Mock<IDishCommand>();
            var mockQuery = new Mock<IDishQuery>();
            var mockMapper = new Mock<IDishMapper>();
            var mockValidator = new Mock<IDishValidator>();
            var service = new DishServices(
                mockCommand.Object,
                mockQuery.Object,
                mockMapper.Object,
                mockValidator.Object
            );

            var dishEntities = new List<Domain.Entities.Dish>
            {
                new Domain.Entities.Dish
                {
                    ID = Guid.NewGuid(),
                    Name = "test name 1",
                    Description = "test description 1",
                    Price = 1244,
                    CategoryId = 1,
                    ImageURL = "test URL 1"
                },
                new Domain.Entities.Dish
                {
                    ID = Guid.NewGuid(),
                    Name = "test name 2",
                    Description = "test description 2",
                    Price = 2244,
                    CategoryId = 2,
                    ImageURL = "test URL 2"
                }
            };

            var expectedResponses = dishEntities.Select(d => new DishResponse
            {
                ID = d.ID,
                name = d.Name,
                Description = d.Description,
                Price = d.Price,
                Image = d.ImageURL,
                category = new GenericResponse { id = d.CategoryId }
            }).ToList();

            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<string?>()))
                     .ReturnsAsync(dishEntities);
            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>()))
                      .Returns((Domain.Entities.Dish d) => expectedResponses.First(er => er.ID == d.ID));

            // ACT
            var result = await service.GetAllDish(name: "test", categoryId: null, onlyActive: true, sortByPrice: "asc");

            // ASSERT
            result.Should().HaveCount(dishEntities.Count);
            result.Should().BeEquivalentTo(expectedResponses);
        }

        [Fact]
        public async Task UpdateDish_ValidDish_ReturnDishResponse()
        {
            // ARRANGE
            var mockCommand = new Mock<IDishCommand>();
            var mockQuery = new Mock<IDishQuery>();
            var mockMapper = new Mock<IDishMapper>();
            var mockValidator = new Mock<IDishValidator>();
            
            var service = new DishServices(
                mockCommand.Object,
                mockQuery.Object,
                mockMapper.Object,
                mockValidator.Object
            );

            var dishId = Guid.NewGuid();

            var dishRequest = new DishRequest
            {
                Name = "test name update",
                Description = "test description update",
                Price = 1244,
                Category = 1,
                Image = "test URL"
            };

            var dishEntity = new Domain.Entities.Dish
            {
                Name = "test name",
                Description = "test description",
                Price = 1244,
                CategoryId = 2,
                ImageURL = "test URL",
                IsAvailable = true
            };

            var expectedResponse = new DishResponse
            {
                name = dishRequest.Name,
                Description = dishRequest.Description,
                Price = dishRequest.Price,
                Image = dishRequest.Image,
                category = new GenericResponse { id = dishRequest.Category },
                IsActive = true
            };

            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync(dishEntity);
            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>())).Returns(expectedResponse);

            // ACT
            var result = await service.UpdateDish(dishId, dishRequest);

            // ASSERT
            mockValidator.Verify(v => v.ValidateUpdate(dishId, dishRequest), Times.Once);
            mockCommand.Verify(c => c.UpdateDish(dishEntity, dishRequest), Times.Once);
            result.name.Should().Be(expectedResponse.name);
            result.Description.Should().Be(expectedResponse.Description);
            result.Price.Should().Be(expectedResponse.Price);
            result.Image.Should().Be(expectedResponse.Image);
            result.category.id.Should().Be(expectedResponse.category.id);
            result.ID.Should().Be(expectedResponse.ID);
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async  Task CreateDish_ReturnInvalidDishPriceException()
        {
            // ARRANGE
            var mockCommand = new Mock<IDishCommand>();
            var mockQuery = new Mock<IDishQuery>();
            var mockMapper = new Mock<IDishMapper>();
            var mockValidator = new Mock<IDishValidator>();

            var service = new DishServices(
                mockCommand.Object,
                mockQuery.Object,
                mockMapper.Object,
                mockValidator.Object
            );

            var dishRequest = new DishRequest
            {
                Name = "test name",
                Description = "test description",
                Price = -1244,
                Category = 1,
                Image = "test URL"
            };

            mockValidator.Setup(v => v.ValidateCreate(It.IsAny<DishRequest>())).Throws(new InvalidDishPriceException("Precio  invalido"));

            // ACT
            var result = service.CreateDish(dishRequest);

            // ASSERT
            await Assert.ThrowsAsync<InvalidDishPriceException>(() => result);
        }
    }
}
