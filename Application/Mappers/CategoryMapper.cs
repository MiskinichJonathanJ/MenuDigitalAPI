using Application.DataTransfers.Response.Category;
using Application.Interfaces.ICategory;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class CategoryMapper : ICategoryMapper
    {
        public CategoryResponse ToResponse(Category category)
        {
            CategoryResponse categorySend = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Order = category.Order
            };

            return categorySend;    
        }
    }
}
