using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.DataTransfers.Response.Order;
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
        Task<OrderCreateResponse> CreateOrder(OrderRequest orderCreate);
        Task<ICollection<OrderDetailsResponse>> GetAllOrders(DateTime? desde = null, DateTime? hasta = null, int? statusId = null);
        Task<OrderDetailsResponse> GetOrderById(int orderId);
        Task<OrderUpdateResponse> UpdateStatusItemOrder(int orderId, int itemId, OrderItemUpdateRequest request);
    }
}
