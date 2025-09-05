using Application.DataTransfers.Request.Dish;
using Application.Interfaces.DishInterfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Command
{
    public class DishCommand : IDishCommand
    {
        private readonly AppDbContext _context;

        public DishCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateDish(Dish dish)
        {
            _context.Add(dish);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteDish(Dish dish)
        {
            _context.Remove(dish);

            await _context.SaveChangesAsync();
        }

        public async  Task UpdateDish(Dish dishEnDB, UpdateDishRequest dishActualizado)
        {

            dishEnDB.Name = dishActualizado.Name;
            dishEnDB.Description = dishActualizado.Description;
            dishEnDB.Price = dishActualizado.Price;
            dishEnDB.CategoryId = dishActualizado.Category;
            dishEnDB.ImageURL = dishActualizado.Image;
            dishEnDB.UpdatedDate = DateTime.UtcNow;
            dishEnDB.IsAvailable = dishActualizado.IsActive;

            await _context.SaveChangesAsync();
        }
    }
}
