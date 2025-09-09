using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IDish
{
    public interface IDishQuery
    {
        Task<ICollection<Domain.Entities.Dish>> GetAllDish(
            string? name = null,
            int? categoryId = null,
            bool? onlyActive = null,
            string? sortByPrice = null
        );
        Task<Domain.Entities.Dish?> GetDishById(Guid dishId);
        Task<Category?> GetCategoryById(int id);
    }
}
