using Application.DataTransfers.Response;
using Application.Interfaces.IDeliveryType;
using Microsoft.AspNetCore.Mvc;

namespace MenuDigitalRestaurante.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DeliveryTypeController : ControllerBase
    {
        public readonly IDeliveryTypeService _service;
        public DeliveryTypeController(IDeliveryTypeService services)
        {
            _service = services;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GenericResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDeliveryTypes()
        {
            var result = await _service.GetAllDeliveryTypes();
            return Ok(result);
        }
    }
}
