using Application.DataTransfers.Response;
using Domain.Entities;

namespace Application.Interfaces.IStatus
{
    public interface IStatusMapper
    {
        GenericResponse ToResponse(Status status);
    }
}
