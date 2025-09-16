using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.DishException
{
    public class DishNameTooLongException : ValidationException
    {
        public DishNameTooLongException() : base("El nombre del platillo no debe exceder los 256 caracteres")
        {
        }
    }
}
