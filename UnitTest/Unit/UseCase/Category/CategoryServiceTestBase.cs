using Application.Interfaces.ICategory;
using Application.UseCase.Category;
using Moq;

namespace UnitTest.Unit.UseCase.CategoryTest
{
    public class CategoryServiceTestBase
    {
        protected Mock<ICategoryQuery> mockQuery = new();
        protected Mock<ICategoryMapper> mockMapper = new();
        protected CategoryService service;

        protected CategoryServiceTestBase() 
        {
            mockQuery = new Mock<ICategoryQuery>(MockBehavior.Strict);
            mockMapper = new Mock<ICategoryMapper>(MockBehavior.Strict);
            service = new CategoryService(
                mockQuery.Object,
                mockMapper.Object
            );
        }

        protected void VerifyNoOtherCalls()
        {
            mockQuery.VerifyNoOtherCalls();
            mockMapper.VerifyNoOtherCalls();
        }
    }
}
