using Domain.Entities;

namespace Application.Interfaces.IStatus
{
    public interface IStatusQuery
    {
        Task<ICollection<Status>> GetAllStatuses();
    }
}
