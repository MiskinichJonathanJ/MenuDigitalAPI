using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response.Dish;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IDish
{
    public interface IDishMapper
    {
        Domain.Entities.Dish ToEntity(DishRequest request);
        DishResponse ToResponse(Domain.Entities.Dish dish);
    }
}
