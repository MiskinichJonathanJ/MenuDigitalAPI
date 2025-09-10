using Application.DataTransfers.Response.Category;
using Domain.Entities;
using Moq;

namespace UnitTest.Unit.UseCase.CategoryTest
{
    public class CategoryServiceTest : CategoryServiceTestBase
    {
        [Fact]
        public async Task GetAllCategories_ReturnMappedCategories()
        {
            // ARRANGE
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", Description = "test"},
                new Category { Id = 2, Name = "Category 2", Description = "test"}
            };
            mockQuery.Setup(q => q.GetAllCategories())
                     .ReturnsAsync(categories);
            mockMapper.Setup(m => m.ToResponse(It.Is<Domain.Entities.Category>(c => c.Id == 1)))
                      .Returns(new CategoryResponse { Id = 1, Name = "Category 1", Description = "test" });
            mockMapper.Setup(m => m.ToResponse(It.Is<Domain.Entities.Category>(c => c.Id == 2)))
                      .Returns(new CategoryResponse {Id = 2, Name = "Category 2", Description = "test" });
            // ACT
            var result = await service.GetAllCategories();
            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Id == 1 && r.Name == "Category 1");
            Assert.Contains(result, r => r.Id == 2 && r.Name == "Category 2");

            mockQuery.Verify(q => q.GetAllCategories(), Times.Once);
            mockMapper.Verify(m => m.ToResponse(It.IsAny<Domain.Entities.Category>()), Times.Exactly(2));
            VerifyNoOtherCalls();
        }
    }
}
