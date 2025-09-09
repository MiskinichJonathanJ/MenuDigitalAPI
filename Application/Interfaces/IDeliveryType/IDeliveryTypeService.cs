using Application.DataTransfers.Response;

namespace Application.Interfaces.IDeliveryType
{
    public interface IDeliveryTypeService
    {
        Task<ICollection<GenericResponse>> GetAllDeliveryTypes();
    }
}
