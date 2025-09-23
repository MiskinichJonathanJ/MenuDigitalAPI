using Application.Exceptions;
using Application.Exceptions.CategoryException;
using Application.Exceptions.DishException;
using Application.Validations;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;

namespace UnitTest.Unit.Validations
{
    public class DishValidatorTest : DishValidatorTestBase
    {
        [Fact]
        public void ValidateCommon_ValidParams_ReturnsNothinException()
        {
            // ARRANGE
            var validRequest = BuildValidBaseRequest();

            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });

            // ACT & ASSERT
           FluentActions.Invoking(() => validator.ValidateCommon(validRequest)).Should().NotThrow();
        }
        public static IEnumerable<object[]> ValidateMethodsTestData =>
            new List<object[]>
            {
                new object[]
                {
                    () => new DishValidatorTest().validator.ValidateCreate(null!)
                },
                new object[]
                {
                    () => new DishValidatorTest().validator.ValidateUpdate(Guid.NewGuid(), null!)
                }
            };
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void ValidateCommon_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // ARRANGE
            var invalidRequest = BuildValidBaseRequest();
            invalidRequest.Name = invalidName;
            // ACT & ASSERT
            Assert.Throws<DishNameIsNullException>(() => validator.ValidateCommon(invalidRequest));
        }

        [Fact]
        public void ValidateCommon_NameExceedsMaxLength_ThrowsArgumentException()
        {
            // ARRANGE
            var invalidRequest = BuildValidBaseRequest();
            invalidRequest.Name = new string('a', 256);

            // ACT & ASSERT
            Assert.Throws<DishNameTooLongException>(() => validator.ValidateCommon(invalidRequest));
        }

        [Fact]
        public void ValidateCommon_InvalidPrice_ThrowsInvalidDishPriceException()
        {
            // ARRANGE
            var invalidRequest = BuildValidBaseRequest();
            invalidRequest.Price = 0;
            // ACT & ASSERT
            Assert.Throws<InvalidDishPriceException>(() => validator.ValidateCommon(invalidRequest));
        }
        [Fact]
        public void ValidateCommon_WithNullRequest_ThrowsArgumentNullException()
        {
            // ACT & ASSERT
            FluentActions.Invoking(() => validator.ValidateCommon(null!))
                .Should().Throw<RequestNullException>();
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
            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>())).ReturnsAsync([new Domain.Entities.Dish() { Name = "test", Description = "test", ImageUrl = "URL"}]);

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

            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Dish() { DishId = guid, Name = "test", Description = "test", ImageUrl = "URL" });
            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });
            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>())).ReturnsAsync([new Domain.Entities.Dish() { DishId = guid, Name = "test", Description = "test", ImageUrl = "URL" }]);

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

            mockQuery.Setup(q => q.GetDishById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Dish() { DishId = guid, Name = "test", Description = "test", ImageUrl = "URL" });
            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Category { Id = 1, Name = "test", Description = "test" });
            mockQuery.Setup(q => q.GetAllDish(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string?>())).ReturnsAsync([new Domain.Entities.Dish() {DishId=Guid.NewGuid(), Name = "test", Description = "test", ImageUrl = "URL" }]);

            // ACT
            Func<Task> act = async () => await validator.ValidateUpdate(guid, validRequest);

            // ASSERT
            await act.Should().ThrowAsync<DishNameAlreadyExistsException>();
        }

        [Fact]
        public async Task ValidateCategoryExist_WithCategoryNull_ThrowsCategoryNotFoundException()
        {
            // ARRANGES
            mockQuery.Setup(q => q.GetCategoryById(It.IsAny<int>())).ReturnsAsync((Category?)null);
            // ACT  
            Func<Task> act = async () => await validator.ValidateCategoryExists(1);
            // ASSERT
            await act.Should().ThrowAsync<CategoryNotFoundException>();
        }

        [Theory]
        [MemberData(nameof(ValidateMethodsTestData))]
        public async Task ValidateMethods_WithNullRequest_ThrowsRequestNullException(Func<Task> action)
        {
            // ACT & ASSERT
            await FluentActions.Invoking(action)
                .Should().ThrowAsync<RequestNullException>();
        }
    }
}
