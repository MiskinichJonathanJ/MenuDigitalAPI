using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.OrderException
{
    public class InvalidOrderIdException : OrderRequestException
    {
        public InvalidOrderIdException() : base("El Id de la orden es invalido") { }
    }
}
