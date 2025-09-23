using Application.DataTransfers.Response;
using Application.Interfaces.IStatus;
using Domain.Entities;

namespace Application.Mappers
{
    public class StatusMapper : IStatusMapper
    {
        public GenericResponse ToResponse(Status status)
        {
            return new GenericResponse
            {
                id = status.Id,
                name = status.Name
            };
        }
    }
}
