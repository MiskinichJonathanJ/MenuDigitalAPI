using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response;
using Application.DataTransfers.Response.Dish;
using Application.Interfaces.IDish;
using Application.UseCase.DishUse;
using Moq;

namespace UnitTest.Unit.UseCase.Dish
{
    public abstract class DishServiceTestBase
    {
        // ARRANGE
        protected Mock<IDishCommand> mockCommand = new();
        protected Mock<IDishQuery> mockQuery = new();
        protected Mock<IDishMapper> mockMapper = new();
        protected Mock<IDishValidator> mockValidator = new();
        protected DishServices service;

        protected DishServiceTestBase()
        {
            mockCommand = new Mock<IDishCommand>(MockBehavior.Strict);
            mockQuery = new Mock<IDishQuery>(MockBehavior.Strict);
            mockMapper = new Mock<IDishMapper>(MockBehavior.Strict);
            mockValidator = new Mock<IDishValidator>(MockBehavior.Strict);

            service = new DishServices(
                mockCommand.Object,
                mockQuery.Object,
                mockMapper.Object,
                mockValidator.Object
            );
        }

        protected DishRequest BuildValidRequest() => new()
        {
            Name = "test name",
            Description = "test description",
            Price = 1244,
            Category = 1,
            Image = "test URL"
        };

        protected static Domain.Entities.Dish BuildEntity(DishRequest req) => new()
        {
            Name = req.Name,
            Description = req.Description,
            Price = req.Price,
            CategoryId = req.Category,
            ImageURL = req.Image
        };

        protected static DishResponse BuildResponse(Domain.Entities.Dish entity) => new()
        {
            name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Image = entity.ImageURL,
            category = new GenericResponse { id = entity.CategoryId, name = "Category Name" }
        };
        protected void VerifyNoOtherCalls()
        {
            mockCommand.VerifyNoOtherCalls();
            mockQuery.VerifyNoOtherCalls();
            mockMapper.VerifyNoOtherCalls();
            mockValidator.VerifyNoOtherCalls();
        }
    }
}
