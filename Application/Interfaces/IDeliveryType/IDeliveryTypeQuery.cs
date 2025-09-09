using Domain.Entities;

namespace Application.Interfaces.IDeliveryType
{
    public interface IDeliveryTypeQuery
    {
        Task<ICollection<DeliveryType>> GetAllDeliveryTypes();
    }
}
