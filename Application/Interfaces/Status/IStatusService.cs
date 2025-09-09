using Application.DataTransfers.Response;

namespace Application.Interfaces.Status
{
    public interface IStatusService
    {
        Task<ICollection<GenericResponse>> GetAllStatus();
    }
}
