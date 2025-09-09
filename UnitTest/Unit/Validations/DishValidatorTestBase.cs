using Application.DataTransfers.Request.Dish;
using Application.Interfaces.DishInterfaces;
using Application.Validations;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
