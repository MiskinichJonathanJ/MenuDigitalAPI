using Application.DataTransfers.Response.Category;
using Domain.Entities;

namespace Application.Interfaces.ICategory
{
    public interface ICategoryMapper
    {
        CategoryResponse ToResponse(Category category);
    }
}
