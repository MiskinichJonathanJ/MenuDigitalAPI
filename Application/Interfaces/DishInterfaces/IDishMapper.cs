using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.DishInterfaces
{
    public interface IDishMapper
    {
        Dish ToEntity(DishRequest request);
        DishResponse ToResponse(Dish dish);
    }
}
