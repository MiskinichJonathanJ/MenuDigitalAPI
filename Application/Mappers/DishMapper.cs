using Application.DataTransfers.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.DishInterfaces;
using Application.DataTransfers.Request.Dish;

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
                IsAvailable = true,
                ImageURL = request.Image,
                CategoryId = request.Category,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            return dish;
        }

        public DishResponse ToResponse(Dish dish)
        {
            var dishResponse = new DishResponse
            {
                ID = dish.ID,
                name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                IsActive = dish.IsAvailable,
                Image = dish.ImageURL,
                CreatedAt = dish.CreatedDate,
                UpdatedAt = dish.UpdatedDate,
                category = new GenericResponse
                {
                    id = dish.CategoryId,
                    name = dish.CategoryNav.Name
                }
            };
            return dishResponse;
        }
    }
}
