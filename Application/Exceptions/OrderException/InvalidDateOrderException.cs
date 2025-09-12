using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.OrderException
{
    internal class InvalidDateOrderException : OrderRequestException
    {
        public InvalidDateOrderException() : base("El rango de fechar es invalido") { }
    }
}
