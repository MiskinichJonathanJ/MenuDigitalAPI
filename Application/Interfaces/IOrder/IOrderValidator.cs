using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
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
        Task ValidateGetAllOrders(DateTime? desde = null, DateTime? hasta = null, int? statusId = null);
        Task ValidateGetOrderById(int orderId);
        Task ValidateUpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request);
    }
}
