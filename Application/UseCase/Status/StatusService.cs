using Application.DataTransfers.Response;
using Application.Interfaces.IStatus;
using Domain.Entities;

namespace Application.UseCase.StatusUse
{
    public class StatusService : IStatusService
    {
        private readonly IStatusQuery _query;
        private readonly IStatusMapper _mapper;
        public StatusService(IStatusQuery query, IStatusMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }
        public async Task<ICollection<GenericResponse>> GetAllStatus()
        {
            IEnumerable<Status> statuses = await _query.GetAllStatuses();
            return [.. statuses.Select(s => _mapper.ToResponse(s))];
        }
    }
}
