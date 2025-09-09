using Application.DataTransfers.Request.Dish;
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
        public async Task UpdateDish_ValidRequest_ReturnDishResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            var validRequest = BuildValidRequest();
            var dishEntity = BuildEntity(validRequest);
            var dishRequest = new UpdateDishRequest
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

            mockValidator.Setup(v => v.ValidateUpdate(It.IsAny<Guid>(), It.IsAny<UpdateDishRequest>())).Returns(Task.CompletedTask);
            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync(dishEntity);
            mockCommand.Setup(c => c.UpdateDish(It.IsAny<Domain.Entities.Dish>(), It.IsAny<UpdateDishRequest>())).Returns(Task.CompletedTask);
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
            result.category.Should().NotBeNull();
            result.category.id.Should().Be(expectedResponse.category.id);
            result.ID.Should().Be(expectedResponse.ID);
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateDish_InvalidIdRequest_ReturnDishResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            Domain.Entities.Dish? dishEntity = null;
            var validRequest = BuildValidRequest();
            var dishRequest = new UpdateDishRequest
            {
                Name = validRequest.Name,
                Description = validRequest.Description,
                Price = validRequest.Price,
                Category = validRequest.Category,
                Image = validRequest.Image,
                IsActive = true
            };

            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync(dishEntity);

            //Assert
            await Assert.ThrowsAsync<DishNotFoundException>(() => service.UpdateDish(dishId, dishRequest));
            mockQuery.Verify(q => q.GetDishById(dishId), Times.Once);
            VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAllDish_WhenCalled_ReturnsMappedResponses()
        {
            // ARRANGE
            var dishes = new List<Domain.Entities.Dish>
            {
                new Domain.Entities.Dish { ID = Guid.NewGuid(), Name = "Pizza Napoletana", Description  = "test", ImageURL= "URL", CategoryId = 2 },
                new Domain.Entities.Dish { ID = Guid.NewGuid(), Name = "Pizza Margherita", Description  = "test", ImageURL= "URL", CategoryId = 2 }
            };
            mockQuery.Setup(q => q.GetAllDish("Pizza", 2, true, "asc"))
                     .ReturnsAsync(dishes);

            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>()))
                      .Returns<Domain.Entities.Dish>(d => new DishResponse { name = d.Name, Description = d.Description, Image = d.ImageURL });

            // ACT
            var result = await service.GetAllDish("Pizza", 2, true, "asc");

            // ASSERT
            mockQuery.Verify(q => q.GetAllDish("Pizza", 2, true, "asc"), Times.Once);
            mockMapper.Verify(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>()), Times.Exactly(dishes.Count));
            VerifyNoOtherCalls();

            result.Should().HaveCount(2);
            result.Select(r => r.name).Should().Contain(["Pizza Napoletana", "Pizza Margherita"]);
        }

        [Fact]
        public async Task GetDishById_WhenDishExists_ReturnsMappedResponse()
        {
            // ARRANGE
            var dishId = Guid.NewGuid();
            var validRequest = BuildValidRequest();
            var dishEntity = BuildEntity(validRequest);
            dishEntity.ID = dishId;
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
            result.category.id.Should().Be(expectedResponse.category.id);
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
            dishEntity.ID = dishId;

            mockQuery.Setup(q => q.GetDishById(dishId)).ReturnsAsync(dishEntity);
            mockCommand.Setup(c => c.DeleteDish(dishEntity)).Returns(Task.CompletedTask);
            mockMapper.Setup(m => m.ToResponse(It.IsAny<Domain.Entities.Dish>())).Returns(BuildResponse(dishEntity));

            // ACT
            await service.DeleteDish(dishId);

            // ASSERT
            mockQuery.Verify(q => q.GetDishById(dishId), Times.Once);
            mockCommand.Verify(c => c.DeleteDish(dishEntity), Times.Once);
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
