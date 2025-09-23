using Application.DataTransfers.Response;
using Domain.Entities;
using Application.Interfaces.IDish;
using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response.Dish;

namespace Application.Mappers
{
    public class DishMapper : IDishMapper
    {
        public Dish ToEntity(DishRequest request)
        {
            var dish = new Dish
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Available = true,
                ImageUrl = request.Image,
                Category = request.Category,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };
            return dish;
        }

        public DishResponse ToResponse(Dish dish)
        {
            var dishResponse = new DishResponse
            {
                ID = dish.DishId,
                name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                IsActive = dish.Available,
                Image = dish.ImageUrl,
                CreatedAt = dish.CreateDate,
                UpdatedAt = dish.UpdateDate,
                category = new GenericResponse
                {
                    id = dish.Category,
                    name = dish.CategoryNav?.Name ?? "Sin categoría"
                }
            };
            return dishResponse;
        }
    }
}
