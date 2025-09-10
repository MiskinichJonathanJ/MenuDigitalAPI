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
        public  readonly IOrderService _orderService;
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
    }
}
