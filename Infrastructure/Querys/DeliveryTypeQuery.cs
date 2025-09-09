using Application.Interfaces.IDeliveryType;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class DeliveryTypeQuery : IDeliveryTypeQuery
    {
        public  readonly AppDbContext _context;
        public DeliveryTypeQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<DeliveryType>> GetAllDeliveryTypes()
        {
            IQueryable<DeliveryType> query = _context.DeliveryTypes;
            return await query.ToListAsync();
        }
    }
}
