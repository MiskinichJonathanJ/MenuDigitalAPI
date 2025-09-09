using Application.DataTransfers.Response.Category;
using Application.Interfaces.ICategory;

namespace Application.UseCase.Category
{
    public class CategoryService : ICategoryService
    {
        public readonly ICategoryQuery _query;
        public readonly ICategoryMapper _mapper;

        public  CategoryService(ICategoryQuery query, ICategoryMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }
        public  async Task<ICollection<CategoryResponse>> GetAllCategories()
        {
            var categories = await _query.GetAllCategories();
            return categories.Select(c => _mapper.ToResponse(c)).ToList();
        }
    }
}
