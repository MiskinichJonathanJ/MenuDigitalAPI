using Application.Interfaces.ICategory;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class CategoryQuery : ICategoryQuery
    {
        private readonly AppDbContext _context;
        public CategoryQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Category>> GetAllCategories()
        {
            IQueryable<Category> query = _context.Category;
            return await query.ToListAsync();
        }
    }
}
