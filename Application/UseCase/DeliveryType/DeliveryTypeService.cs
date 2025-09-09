using Application.DataTransfers.Response;
using Application.Interfaces.IDeliveryType;

namespace Application.UseCase.DeliveryTypeUse
{
    public class DeliveryTypeService : IDeliveryTypeService
    {
        public  readonly IDeliveryTypeQuery _query;
        public readonly IDeliveryTypeMapper _mapper;
        public DeliveryTypeService(IDeliveryTypeQuery query, IDeliveryTypeMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }
        public async Task<ICollection<GenericResponse>> GetAllDeliveryTypes()
        {
            var deliveryTypes = await _query.GetAllDeliveryTypes();
            return [.. deliveryTypes.Select(dt => _mapper.ToResponse(dt))];
        }
    }
}
