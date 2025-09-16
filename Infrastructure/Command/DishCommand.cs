using Application.DataTransfers.Request.Dish;
using Application.Exceptions.DishException;
using Application.Interfaces.IDish;
using Application.Validations.Helpers;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static Application.Validations.Helpers.OrderItemStatusFlow;

namespace Infrastructure.Command
{
    public class DishCommand : IDishCommand
    {
        private readonly AppDbContext _context;

        public DishCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dish> CreateDish(Dish dish)
        {
            var addedEntity = _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public async Task DeleteDish(Guid dishId)
        {
            Dish dish = await _context.Dishes.Where(d => d.ID == dishId).Include(d =>  d.OrderItems).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Dish not found");
            if (dish.OrderItems.Count == 0)
                _context.Dishes.Remove(dish);
            else if (dish.OrderItems.All(oi => oi.StatusId == (int)OrderItemStatus.Closed))
                dish.IsAvailable = false;
            else
                throw new InvlidDeleteDishException();

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
