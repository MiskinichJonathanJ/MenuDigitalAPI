using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IOrder;
using Application.DataTransfers.Request.Order;

namespace MenuDigitalRestaurante.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : Controller
    {
        public readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var result = await _orderService.CreateOrder(request);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null,
            [FromQuery] int? statusId = null
            )
        {
            var result = await _orderService.GetAllOrders(desde, hasta, statusId);
            return new JsonResult(result);
        }
    }
}
