using Application.DataTransfers.Response.Category;

namespace Application.Interfaces.ICategory
{
    public interface ICategoryService
    {
        Task<ICollection<CategoryResponse>> GetAllCategories();
    }
}
