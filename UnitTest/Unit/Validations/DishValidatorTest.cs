using Application.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Unit.Validations
{
    public class DishValidatorTest : DishValidatorTestBase
    {
        [Fact]
        public async Task ValidateCommon_ValidParams_ReturnsNothinException()
        {
            // ARRANGE
            var validRequest = BuildValidBaseRequest();

            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });

            // ACT
            Func<Task> act = async () => await validator.ValidateCommon(validRequest);

            // ASSERT
            await act.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ValidateCommon_InvalidName_ThrowsArgumentException()
        {
            // ARRANGE
            var invalidRequest = BuildValidBaseRequest();
            invalidRequest.Name = "";
            // ACT
            Func<Task> act = async () => await validator.ValidateCommon(invalidRequest);
            // ASSERT
            await act.Should().ThrowAsync<ArgumentException>()
                     .WithMessage("El nombre del platillo no puede estar vacío");
        }

        [Fact]
        public async Task ValidateCommon_NameExceedsMaxLength_ThrowsArgumentException()
        {
            // ARRANGE
            var invalidRequest = BuildValidBaseRequest();
            invalidRequest.Name = new string('a', 256);

            // ACT
            Func<Task> act = async () => await validator.ValidateCommon(invalidRequest);
            
            // ASSERT
            await act.Should().ThrowAsync<ArgumentException>()
                     .WithMessage("El nombre del platillo no debe exceder los 256 caracteres");
        }

        [Fact]
        public async Task ValidateCommon_InvalidPrice_ThrowsInvalidDishPriceException()
        {
            // ARRANGE
            var invalidRequest = BuildValidBaseRequest();
            invalidRequest.Price = 0;
            // ACT
            Func<Task> act = async () => await validator.ValidateCommon(invalidRequest);
            // ASSERT
            await act.Should().ThrowAsync<Application.Exceptions.InvalidDishPriceException>()
                     .WithMessage("El precio del platillo debe ser mayor a 0");
        }

        [Fact]
        public async Task ValidateCommon_NonExistentCategory_ThrowsCategoryNotFoundException()
        {
            // ARRANGE
            var invalidRequest = BuildValidBaseRequest();
            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Category?)null);
            // ACT
            Func<Task> act = async () => await validator.ValidateCommon(invalidRequest);
            // ASSERT
            await act.Should().ThrowAsync<Application.Exceptions.CategoryNotFoundException>()
                     .WithMessage("La categoria no existe");
        }

        [Fact]
        public async Task ValidateCreate_ValidParams_ReturnsNothingException()
        {
            // ARRANGE
            var validRequest = BuildValidBaseRequest();
            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });
            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>())).ReturnsAsync(new List<Domain.Entities.Dish>());
            
            // ACT
            Func<Task> act = async () => await validator.ValidateCreate(validRequest);
           
            // ASSERT
            await act.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ValidateCreate_InvalidParams_ReturnsDishNameAlreadyExistsException()
        {
            // ARRANGE
            var validRequest = BuildValidBaseRequest();

            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });
            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>())).ReturnsAsync([new Domain.Entities.Dish() { Name = "test", Description = "test", ImageURL = "URL"}]);

            // ACT
            Func<Task> act = async () => await validator.ValidateCreate(validRequest);

            // ASSERT
            await act.Should().ThrowAsync<DishNameAlreadyExistsException>();
        }

        [Fact]
        public async Task ValidateUpdate_ValidParams_ReturnsNothingException()
        {
            // ARRANGE
            var validRequest = BuildValidUpdateRequest();
            Guid guid = Guid.NewGuid();

            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Dish() { ID = guid, Name = "test", Description = "test", ImageURL = "URL" });
            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });
            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>())).ReturnsAsync([new Domain.Entities.Dish() { ID = guid, Name = "test", Description = "test", ImageURL = "URL" }]);

            // ACT
            Func<Task> act = async () => await validator.ValidateUpdate(guid, validRequest);

            // ASSERT
            await act.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ValidateUpdate_InvalidParams_ReturnsDishNameAlreadyExistsException()
        {
            // ARRANGE
            var validRequest = BuildValidUpdateRequest();
            Guid guid = Guid.NewGuid();

            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Dish() { ID = guid, Name = "test", Description = "test", ImageURL = "URL" });
            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });
            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>())).ReturnsAsync([new Domain.Entities.Dish() {ID=Guid.NewGuid(), Name = "test", Description = "test", ImageURL = "URL" }]);

            // ACT
            Func<Task> act = async () => await validator.ValidateUpdate(guid, validRequest);

            // ASSERT
            await act.Should().ThrowAsync<DishNameAlreadyExistsException>();
        }

        [Fact]
        public async Task ValidateUpdate_InvalidParams_ReturnsDishNotFoundException()
        {
            // ARRANGE
            var validRequest = BuildValidUpdateRequest();

            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Dish?)null);

            // ACT
            Func<Task> act = async () => await validator.ValidateUpdate(Guid.NewGuid(), validRequest);

            // ASSERT
            await act.Should().ThrowAsync<DishNotFoundException>();
        }

    }
}
