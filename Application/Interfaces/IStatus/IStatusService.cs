using Application.DataTransfers.Response;

namespace Application.Interfaces.IStatus
{
    public interface IStatusService
    {
        Task<ICollection<GenericResponse>> GetAllStatus();
    }
}
