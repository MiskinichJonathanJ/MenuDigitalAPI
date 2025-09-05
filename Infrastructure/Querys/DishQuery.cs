using Application.Interfaces.DishInterfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Querys
{
    public class DishQuery : IDishQuery
    {
        public readonly AppDbContext _context;

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
            IQueryable<Dish> query = _context.Dishes.Include(d =>  d.CategoryNav);
            
            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => d.Name.Equals(name));
            
            if(categoryId.HasValue)
                query = query.Where(d => d.CategoryId == categoryId);

            if(onlyActive != null)
                query = query.Where(d => d.IsAvailable == onlyActive);
            
            if (!string.IsNullOrEmpty(sortByPrice))
            {
                query = sortByPrice.ToLower() switch
                {
                    "asc" => query.OrderBy(d => d.Price),
                    "desc" => query.OrderByDescending(d => d.Price),
                    _ => throw new ArgumentException("Invalid sortByPrice parameter")
                };
            }
            
            return await query.ToListAsync();
        }

        public async Task<Dish?> GetDishById(Guid dishId)
        {
            return await _context.Dishes.Include(d => d.CategoryNav).FirstOrDefaultAsync(d=> d.ID == dishId);
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
