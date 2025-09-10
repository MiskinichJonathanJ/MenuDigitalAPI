using Application.Interfaces.IOrder;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Command
{
    public class OrderCommand : IOrderCommand
    {
        private readonly  AppDbContext _context;
        public OrderCommand(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();
        }

        public Task CreateOrderItems(ICollection<OrderItem> orderItems)
        {
            throw new NotImplementedException();
        }
    }
}
