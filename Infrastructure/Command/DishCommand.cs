using Application.DataTransfers.Request.Dish;
using Application.Exceptions.DishException;
using Application.Interfaces.IDish;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            var addedEntity = _context.Dish.Add(dish);
            await _context.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public async Task DeleteDish(Guid dishId)
        {
            var dish = await _context.Dish.FindAsync(dishId) ?? throw new DishNotFoundException();

            bool hasAnyOrders = await _context.OrderItem
                .AnyAsync(oi => oi.Dish== dishId);

            if (!hasAnyOrders)
            {
                _context.Dish.Remove(dish);
                await _context.SaveChangesAsync();
                return;
            }

            bool hasNonClosed = await _context.OrderItem
                .AnyAsync(oi => oi.Dish == dishId && oi.Status != (int)OrderItemStatus.Closed);

            if (!hasNonClosed)
            {
                dish.Available = false;
                await _context.SaveChangesAsync();
                return;
            }

            throw new InvalidDeleteDishException();
        }

        public async Task<Dish> UpdateDish(Guid id, DishUpdateRequest dishActualizado)
        {
            var dishEnDB = await _context.Dish.FindAsync(id) ?? throw new DishNotFoundException();

            bool exists = await _context.Dish.AnyAsync(d => d.DishId != id && d.Name == dishActualizado.Name);

            if (exists)
                throw new DishNameAlreadyExistsException();

            dishEnDB.Name = dishActualizado.Name;
            dishEnDB.Description = dishActualizado.Description ?? "";
            dishEnDB.Price = dishActualizado.Price;
            dishEnDB.Category = dishActualizado.Category;
            dishEnDB.ImageUrl = dishActualizado.Image ?? "";
            dishEnDB.UpdateDate = DateTime.UtcNow;
            dishEnDB.Available = dishActualizado.IsActive;

            await _context.SaveChangesAsync(); 
            return dishEnDB;
        }
    }
}
