using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder
{
    public interface IOrderCommand
    {
        Task CreateOrder(Order order);
        Task CreateOrderItems(ICollection<OrderItem> orderItems);
    }
}
