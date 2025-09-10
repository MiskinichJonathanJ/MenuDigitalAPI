using Application.DataTransfers.Request.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder
{
    public interface IOrderValidator
    {
       Task ValidateCreateOrder(OrderRequest orderCreate);
    }
}
