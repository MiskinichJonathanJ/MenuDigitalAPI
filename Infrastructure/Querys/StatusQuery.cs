using Application.Interfaces.IStatus;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Querys
{
    public class StatusQuery : IStatusQuery
    {
        public readonly AppDbContext _context;
        public StatusQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<Status>> GetAllStatuses()
        {
            IQueryable<Status> statuses = _context.Statuses;
            return await statuses.ToListAsync();
        }
    }
}
