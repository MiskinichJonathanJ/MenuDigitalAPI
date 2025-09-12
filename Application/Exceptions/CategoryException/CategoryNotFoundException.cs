using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.CategoryException
{
    public class CategoryNotFoundException : NotFoundException { public CategoryNotFoundException() : base("Categoria no encontrada") { } }
}
