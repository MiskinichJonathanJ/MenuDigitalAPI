using Application.Interfaces.IDeliveryType;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class DeliveryTypeQuery : IDeliveryTypeQuery
    {
        private readonly AppDbContext _context;
        public DeliveryTypeQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<DeliveryType>> GetAllDeliveryTypes()
        {
            IQueryable<DeliveryType> query = _context.DeliveryType;
            return await query.ToListAsync();
        }
    }
}
