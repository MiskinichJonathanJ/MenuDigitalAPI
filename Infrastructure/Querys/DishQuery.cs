using Application.Exceptions.DishException;
using Application.Interfaces.IDish;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class DishQuery : IDishQuery
    {
        private readonly AppDbContext _context;

        public DishQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Dish>> GetAllDish(
            string? name = null,
            int? categoryId = null,
            bool? onlyActive = null,
            string? sortByPrice = null
        )
        {
            IQueryable<Dish> query = _context.Dish.Include(d =>  d.CategoryNav);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => EF.Functions.ILike(d.Name, $"%{name}%"));


            if (categoryId.HasValue)
                query = query.Where(d => d.Category == categoryId);

            if (onlyActive == true)
                query = query.Where(d => d.Available);

            if (!string.IsNullOrEmpty(sortByPrice))
            {
                query = sortByPrice.ToLower() switch
                {
                    "asc" => query.OrderBy(d => d.Price),
                    "desc" => query.OrderByDescending(d => d.Price),
                    _ => throw new InvalidSortByPriceException()
                };
            }
            
            return await query.ToListAsync();
        }

        public async Task<Dish?> GetDishById(Guid dishId)
        {
            return await _context.Dish.Include(d => d.CategoryNav).FirstOrDefaultAsync(d=> d.DishId == dishId);
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _context.Category.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
