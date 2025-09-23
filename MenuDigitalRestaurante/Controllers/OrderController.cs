using Application.DataTransfers.Request.Order;
using Application.DataTransfers.Request.OrderItem;
using Application.DataTransfers.Response.Order;
using Application.DataTransfers.Response.OrderResponse;
using Application.Interfaces.IOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Compute.Fluent.Models;

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
        [ProducesResponseType(typeof(OrderCreateResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var result = await _orderService.CreateOrder(request);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int? status = null
            )
        {
            var result = await _orderService.GetAllOrders(from, to, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            var result = await _orderService.GetOrderById(id);
            return Ok(result);
        }

        [HttpPatch("{id}/item/{itemId}")]
        [ProducesResponseType(typeof(OrderUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatusItemOrder([FromRoute] int id, [FromRoute] int itemId, [FromBody] OrderItemUpdateRequest request)
        {
            var result = await _orderService.UpdateStatusItemOrder(id, itemId, request);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(OrderUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrder(OrderUpdateRequest  request, int id)
        {
            var result = await _orderService.UpdateOrder(request,  id);
            return Ok(result);
        }
    }
}
