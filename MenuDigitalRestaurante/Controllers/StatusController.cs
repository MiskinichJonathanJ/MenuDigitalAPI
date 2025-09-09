using Application.DataTransfers.Response;
using Application.Interfaces.Status;
using Microsoft.AspNetCore.Mvc;

namespace MenuDigitalRestaurante.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StatusController : Controller
    {
        public readonly IStatusService _service;
        public StatusController(IStatusService services)
        {
            _service = services;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GenericResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStatus()
        {
            var result = await _service.GetAllStatus();
            return Ok(result);
        }
    }
}
