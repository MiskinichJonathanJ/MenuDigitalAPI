using Application.DataTransfers.Request.Dish;
using Application.Interfaces.IDish;
using Application.Validations;
using Moq;

namespace UnitTest.Unit.Validations
{
    public class DishValidatorTestBase
    {
        protected Mock<IDishQuery> mockQuery = new();
        protected DishValidator validator;
        protected DishValidatorTestBase()
        {
            mockQuery = new Mock<IDishQuery>(MockBehavior.Strict);
            validator = new DishValidator(mockQuery.Object);
        }

        protected DishRequest BuildValidBaseRequest()
        {
            return new DishRequest
            {
                Name = "Pizza",
                Description = "test",
                Image = "test URL",
                Price = 100,
                Category = 1
            };
        }

        protected UpdateDishRequest BuildValidUpdateRequest()
        {
            return new UpdateDishRequest
            {
                Name = "Pizza",
                Description = "test",
                Image = "test URL",
                Price = 100,
                Category = 1,
                IsActive = true
            };
        }
    }
}
