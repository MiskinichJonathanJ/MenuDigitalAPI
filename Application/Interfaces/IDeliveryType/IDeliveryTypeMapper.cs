using Application.DataTransfers.Response;
using Domain.Entities;

namespace Application.Interfaces.IDeliveryType
{
    public interface IDeliveryTypeMapper
    {
        GenericResponse ToResponse(DeliveryType entity);
    }
}
