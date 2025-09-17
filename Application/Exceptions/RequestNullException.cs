using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class RequestNullException : ValidationException
    {
        public RequestNullException() : base("El request enviado es nulo") { }
    }
}
