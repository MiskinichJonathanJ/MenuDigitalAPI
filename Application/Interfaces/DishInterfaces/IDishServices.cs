﻿using Application.DataTransfers.Request.Dish;
using Application.DataTransfers.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.DishInterfaces
{
    public interface IDishServices
    {
        Task<DishResponse> CreateDish(DishRequest request);
        Task<DishResponse> DeleteDish(Guid id);
        Task<ICollection<DishResponse>> GetAllDish(
            string? name = null,
            int? categoryId = null,
            bool? onlyActive = null,
            string? sortByPrice = null
        );
        Task<DishResponse> GetDishById(Guid id);
        Task<DishResponse> UpdateDish(Guid id, UpdateDishRequest request);

    }
}
