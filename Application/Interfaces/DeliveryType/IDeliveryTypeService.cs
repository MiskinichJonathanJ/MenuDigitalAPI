using Application.DataTransfers.Response;

namespace Application.Interfaces.DeliveryType
{
    public interface IDeliveryTypeService
    {
        Task<ICollection<GenericResponse>> GetAllDeliveryTypes();
    }
}
