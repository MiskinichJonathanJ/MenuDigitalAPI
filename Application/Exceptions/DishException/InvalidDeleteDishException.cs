using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.DishException
{
    public class InvalidDeleteDishException : ConflictException
    {
        public InvalidDeleteDishException() : base("No se puded borrar un plato con ordenes activas.") { }
    }
}
