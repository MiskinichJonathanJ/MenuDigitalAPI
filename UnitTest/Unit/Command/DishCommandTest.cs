using Application.DataTransfers.Request.Dish;
using Application.Exceptions.DishException;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Command;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static Application.Validations.Helpers.OrderItemStatusFlow;

namespace UnitTest.Unit.Command
{
    public class DishCommandTest
    {
        private readonly AppDbContext _context;
        private readonly DishCommand _command;

        public DishCommandTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _command = new DishCommand(_context);
        }

        private static Dish CreateDishTest()
        {
            return new Dish
            {
                DishId = Guid.NewGuid(),
                Name = "Pizza Margherita",
                Description = "Classic Italian pizza",
                Price = 15.50m,
                Category = 1,
                ImageUrl = "pizza.jpg",
                Available = true
            };
        }
        [Fact]
        public async Task CreateDish_WithValidDish_ReturnsDishWithGeneratedId()
        {
            // ARRANGE
            var newDish = CreateDishTest();

            // ACT
            var result = await _command.CreateDish(newDish);

            // ASSERT
            result.Should().NotBeNull();
            result.DishId.Should().NotBe(Guid.Empty);
            result.Name.Should().Be("Pizza Margherita");
            result.Description.Should().Be("Classic Italian pizza");
            result.Price.Should().Be(15.50m);
            result.Category.Should().Be(1);
            result.ImageUrl.Should().Be("pizza.jpg");
            result.Available.Should().BeTrue();

            var savedDish = await _context.Dish.FindAsync(result.DishId);
            savedDish.Should().NotBeNull();
            savedDish!.Name.Should().Be("Pizza Margherita");
        }

        [Fact]
        public async Task CreateDish_AddsToContextAndSaves()
        {
            // ARRANGE
            var dishCountBefore = await _context.Dish.CountAsync();
            var newDish = CreateDishTest();

            // ACT
            await _command.CreateDish(newDish);

            // ASSERT
            var dishCountAfter = await _context.Dish.CountAsync();
            dishCountAfter.Should().Be(dishCountBefore + 1);
        }


        [Fact]
        public async Task UpdateDish_WithValidData_UpdatesAllFields()
        {
            // ARRANGE
            var originalDish = CreateDishTest();
            originalDish.CreateDate = DateTime.UtcNow.AddDays(-10);
            originalDish.UpdateDate = DateTime.UtcNow.AddDays(-5);

            _context.Dish.Add(originalDish);
            await _context.SaveChangesAsync();

            var updateRequest = new DishUpdateRequest
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Price = 25.50m,
                Category = 2,
                Image = "updated.jpg",
                IsActive = false
            };

            var timeBefore = DateTime.UtcNow;

            // ACT
            await _command.UpdateDish(originalDish.DishId, updateRequest);

            // ASSERT
            originalDish.Name.Should().Be("Updated Name");
            originalDish.Description.Should().Be("Updated Description");
            originalDish.Price.Should().Be(25.50m);
            originalDish.Category.Should().Be(2);
            originalDish.ImageUrl.Should().Be("updated.jpg");
            originalDish.Available.Should().BeFalse();
            originalDish.UpdateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            var updatedDish = await _context.Dish.FindAsync(originalDish.DishId);
            updatedDish!.Name.Should().Be("Updated Name");
            updatedDish.Price.Should().Be(25.50m);
        }

        [Fact]
        public async Task DeleteDish_WithoutOrderItems_RemovesDishFromDatabase()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems = [];

            _context.Dish.Add(dish);
            await _context.SaveChangesAsync();

            var dishCountBefore = await _context.Dish.CountAsync();

            // ACT
            await _command.DeleteDish(dish.DishId);

            // ASSERT
            var dishCountAfter = await _context.Dish.CountAsync();
            dishCountAfter.Should().Be(dishCountBefore - 1);

            var deletedDish = await _context.Dish.FindAsync(dish.DishId);
            deletedDish.Should().BeNull();
        }


        [Fact]
        public async Task DeleteDish_WithAllClosedOrderItems_SetsIsAvailableToFalse()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems =
            [
                new() { OrderItemId = 1, Status = (int)OrderItemStatus.Closed, Order = 1, Dish = dish.DishId },
                new() { OrderItemId = 2, Status = (int)OrderItemStatus.Closed, Order = 1, Dish = dish.DishId }
            ];

            _context.Dish.Add(dish);
            await _context.SaveChangesAsync();

            // ACT
            await _command.DeleteDish(dish.DishId);

            // ASSERT
            var updatedDish = await _context.Dish.FindAsync(dish.DishId);
            updatedDish.Should().NotBeNull();
            updatedDish!.Available.Should().BeFalse();

            var dishCount = await _context.Dish.CountAsync();
            dishCount.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task DeleteDish_WithActiveOrderItems_ThrowsInvlidDeleteDishException()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems =
                [
                    new() { OrderItemId = 1, Status = (int)OrderItemStatus.Pending,Order = 1, Dish = dish.DishId },
                    new() { OrderItemId = 2, Status = (int)OrderItemStatus.Closed,Order = 1, Dish = dish.DishId }
                ];

            _context.Dish.Add(dish);
            await _context.SaveChangesAsync();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _command.DeleteDish(dish.DishId))
                .Should().ThrowAsync<InvalidDeleteDishException>();


            var unchangedDish = await _context.Dish.FindAsync(dish.DishId);
            unchangedDish.Should().NotBeNull();
            unchangedDish!.Available.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteDish_WithMixedOrderItems_ThrowsInvlidDeleteDishException()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems =
                [
                    new() { OrderItemId = 1, Status = (int)OrderItemStatus.Closed,Order = 1, Dish = dish.DishId },
                    new() { OrderItemId = 2, Status = (int)OrderItemStatus.Preparing,Order = 1, Dish = dish.DishId },
                    new() { OrderItemId = 3, Status = (int)OrderItemStatus.Closed,Order = 1, Dish = dish.DishId }
                ];
            _context.Dish.Add(dish);
            await _context.SaveChangesAsync();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _command.DeleteDish(dish.DishId))
                .Should().ThrowAsync<InvalidDeleteDishException>();
        }

        [Fact]
        public async Task DeleteDish_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // ARRANGE
            var nonExistentId = Guid.NewGuid();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _command.DeleteDish(nonExistentId))
                .Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Dish not found");
        }


        [Fact]
        public async Task DeleteDish_WithEmptyOrderItemsList_RemovesDish()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems = [];

            _context.Dish.Add(dish);
            await _context.SaveChangesAsync();

            // ACT
            await _command.DeleteDish(dish.DishId);

            // ASSERT
            var deletedDish = await _context.Dish.FindAsync(dish.DishId);
            deletedDish.Should().BeNull();
        }

        [Theory]
        [InlineData(OrderItemStatus.Pending)]
        [InlineData(OrderItemStatus.Preparing)]
        [InlineData(OrderItemStatus.Ready)]
        public async Task DeleteDish_WithSingleActiveOrderItem_ThrowsInvlidDeleteDishException(OrderItemStatus activeStatus)
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems =
            [
                new() { OrderItemId = 1, Status = (int)activeStatus, Order = 1, Dish = dish.DishId }
            ];

            _context.Dish.Add(dish);
            await _context.SaveChangesAsync();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _command.DeleteDish(dish.DishId))
                .Should().ThrowAsync<InvalidDeleteDishException>();
        }

        [Fact]
        public async Task UpdateDish_UpdatesOnlyModifiedEntity()
        {
            // ARRANGE
            var dish1 = CreateDishTest();
            var dish2 = CreateDishTest();
            dish2.Name = "Dish 2";

            _context.Dish.AddRange(dish1, dish2);
            await _context.SaveChangesAsync();

            var updateRequest = new DishUpdateRequest { Name = "Updated Dish 1", Description = "New", Price = 15m, Category = 1, Image = "new.jpg", IsActive = true };

            // ACT
            await _command.UpdateDish(dish1.DishId, updateRequest);

            // ASSERT
            var updatedDish1 = await _context.Dish.FindAsync(dish1.DishId);
            var unchangedDish2 = await _context.Dish.FindAsync(dish2.DishId);

            updatedDish1!.Name.Should().Be("Updated Dish 1");
            unchangedDish2!.Name.Should().Be("Dish 2");
        }
    }
}
