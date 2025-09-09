using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataTransfers.Response.Category;

namespace Application.Interfaces.ICategory
{
    public interface ICategoryMapper
    {
        CategoryResponse ToResponse(Category category);
    }
}
