using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.DishException
{
    public class InvalidLengthDIshNameException : ValidationException
    {
        public InvalidLengthDIshNameException() : base("El nombre del platillo no debe exceder los 256 caracteres")
        {
        }
    }
}
