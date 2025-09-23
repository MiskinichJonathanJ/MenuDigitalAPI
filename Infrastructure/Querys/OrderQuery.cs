using Application.DataTransfers.Request.Order;
using Application.Exceptions.OrderException;
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
        public async Task<ICollection<Dish>> GetAllDishesOrder(ICollection<Items> orderItems)
        {
            var orderItemIds = orderItems.Select(oi => oi.Id).ToList();
            IQueryable<Dish> dishes = _context.Dish.Where(d => orderItemIds.Contains(d.DishId) && d.Available);
            return await dishes.ToListAsync();
        }

        public async Task<ICollection<Order>> GetAllOrders(DateTime? desde = null, DateTime? hasta = null, int? statusId = null)
        {
            IQueryable<Order> orders = _context.Order
                .Include(o => o.StatusNav)
                .Include(o => o.DeliveryTypeNav)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.StatusNav)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.DishNav)
                .AsNoTracking();
            if (desde.HasValue)
                orders = orders.Where(o => o.CreateDate >= desde.Value);
            if (hasta.HasValue)
                orders = orders.Where(o => o.CreateDate <= hasta.Value);
            if (statusId.HasValue)
                orders = orders.Where(o => o.OverallStatus == statusId.Value);

            return await orders.ToListAsync();
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Order
                .Include(o => o.StatusNav)
                .Include(o => o.DeliveryTypeNav)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.DishNav)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId) ?? throw new OrderNotFoundException();
        }
    }
}
