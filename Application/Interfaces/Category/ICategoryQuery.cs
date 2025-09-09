using Domain.Entities;

namespace Application.Interfaces.ICategory
{
    public interface ICategoryQuery
    {
        Task<ICollection<Category>> GetAllCategories();
    }
}
