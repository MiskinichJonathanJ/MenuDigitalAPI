using Application.DataTransfers.Request.Order;
using Application.Interfaces.IOrder;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class OrderQuery : IOrderQuery
    {
        private readonly AppDbContext _context;
        public OrderQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Dish>> GetAllDishesOrder(ICollection<ItemRequest> orderItems)
        {
            var orderItemIds = orderItems.Select(oi => oi.Id).ToList();
            IQueryable<Dish> dishes = _context.Dishes.Where(d => orderItemIds.Contains(d.ID) && d.IsAvailable);
            return await dishes.ToListAsync();
        }
    }
}
