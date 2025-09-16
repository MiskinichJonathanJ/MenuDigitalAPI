using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.DishException
{
    public class DishNameIsNullException : ValidationException
    {
        public DishNameIsNullException() : base("El nombre del plato esta vacio.") { }
    }
}
