using Application.DataTransfers.Response.Category;
using Application.Interfaces.ICategory;
using Application.Interfaces.IDish;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Storage.Fluent.Models;

namespace MenuDigitalRestaurante.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoryController : Controller
    {
        public readonly ICategoryService _service;

        public CategoryController(ICategoryService services)
        {
            _service = services;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _service.GetAllCategories();
            return Ok(result);
        }
    }
}
