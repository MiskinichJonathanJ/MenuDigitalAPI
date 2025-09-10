using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Response.OrderResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrder(OrderRequest orderCreate);
    }
}
