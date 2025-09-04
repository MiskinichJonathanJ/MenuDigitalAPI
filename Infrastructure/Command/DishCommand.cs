using Application.DataTransfers.Request;
using Application.Interfaces.DishInterfaces;
using Domain.Entities;
using Infrastructure.Persistence;
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
        public readonly AppDbContext _context;

        public DishCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateDish(Dish dish)
        {
            _context.Add(dish);

            await _context.SaveChangesAsync();
        }

        public Task DeleteDish(Dish dish)
        {
            _context.Remove(dish);

            return _context.SaveChangesAsync();
        }

        public async  Task UpdateDish(Dish dishEnDB, DishRequest dishActualizado)
        { 
            dishEnDB.Name = dishActualizado.Name;
            dishEnDB.Description = dishActualizado.Description;
            dishEnDB.Price = dishActualizado.Price;
            dishEnDB.CategoryId = dishActualizado.Category;
            dishEnDB.ImageURL = dishActualizado.Image;
            dishEnDB.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
