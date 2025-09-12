using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions.OrderException
{
    public class MissingAdrresDeliveryException : OrderRequestException
    {
        public MissingAdrresDeliveryException() : base("La dirección de entrega es obligatoria para pedidos a domicilio.") { }
    }
}
