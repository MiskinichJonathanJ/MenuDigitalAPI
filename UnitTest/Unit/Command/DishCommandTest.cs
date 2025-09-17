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
                ID = Guid.NewGuid(),
                Name = "Pizza Margherita",
                Description = "Classic Italian pizza",
                Price = 15.50m,
                CategoryId = 1,
                ImageURL = "pizza.jpg",
                IsAvailable = true
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
            result.ID.Should().NotBe(Guid.Empty);
            result.Name.Should().Be("Pizza Margherita");
            result.Description.Should().Be("Classic Italian pizza");
            result.Price.Should().Be(15.50m);
            result.CategoryId.Should().Be(1);
            result.ImageURL.Should().Be("pizza.jpg");
            result.IsAvailable.Should().BeTrue();

            var savedDish = await _context.Dishes.FindAsync(result.ID);
            savedDish.Should().NotBeNull();
            savedDish!.Name.Should().Be("Pizza Margherita");
        }

        [Fact]
        public async Task CreateDish_AddsToContextAndSaves()
        {
            // ARRANGE
            var dishCountBefore = await _context.Dishes.CountAsync();
            var newDish = CreateDishTest();

            // ACT
            await _command.CreateDish(newDish);

            // ASSERT
            var dishCountAfter = await _context.Dishes.CountAsync();
            dishCountAfter.Should().Be(dishCountBefore + 1);
        }


        [Fact]
        public async Task UpdateDish_WithValidData_UpdatesAllFields()
        {
            // ARRANGE
            var originalDish = CreateDishTest();
            originalDish.CreatedDate = DateTime.UtcNow.AddDays(-10);
            originalDish.UpdatedDate = DateTime.UtcNow.AddDays(-5);

            _context.Dishes.Add(originalDish);
            await _context.SaveChangesAsync();

            var updateRequest = new UpdateDishRequest
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
            await _command.UpdateDish(originalDish, updateRequest);

            // ASSERT
            originalDish.Name.Should().Be("Updated Name");
            originalDish.Description.Should().Be("Updated Description");
            originalDish.Price.Should().Be(25.50m);
            originalDish.CategoryId.Should().Be(2);
            originalDish.ImageURL.Should().Be("updated.jpg");
            originalDish.IsAvailable.Should().BeFalse();
            originalDish.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            var updatedDish = await _context.Dishes.FindAsync(originalDish.ID);
            updatedDish!.Name.Should().Be("Updated Name");
            updatedDish.Price.Should().Be(25.50m);
        }

        [Fact]
        public async Task DeleteDish_WithoutOrderItems_RemovesDishFromDatabase()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems = [];

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            var dishCountBefore = await _context.Dishes.CountAsync();

            // ACT
            await _command.DeleteDish(dish.ID);

            // ASSERT
            var dishCountAfter = await _context.Dishes.CountAsync();
            dishCountAfter.Should().Be(dishCountBefore - 1);

            var deletedDish = await _context.Dishes.FindAsync(dish.ID);
            deletedDish.Should().BeNull();
        }


        [Fact]
        public async Task DeleteDish_WithAllClosedOrderItems_SetsIsAvailableToFalse()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems =
            [
                new() { Id = 1, StatusId = (int)OrderItemStatus.Closed, OrderId = 1, DishId = dish.ID },
                new() { Id = 2, StatusId = (int)OrderItemStatus.Closed, OrderId = 1, DishId = dish.ID }
            ];

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            // ACT
            await _command.DeleteDish(dish.ID);

            // ASSERT
            var updatedDish = await _context.Dishes.FindAsync(dish.ID);
            updatedDish.Should().NotBeNull();
            updatedDish!.IsAvailable.Should().BeFalse();

            var dishCount = await _context.Dishes.CountAsync();
            dishCount.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task DeleteDish_WithActiveOrderItems_ThrowsInvlidDeleteDishException()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems =
                [
                    new() { Id = 1, StatusId = (int)OrderItemStatus.Pending,OrderId = 1, DishId = dish.ID },
                    new() { Id = 2, StatusId = (int)OrderItemStatus.Closed,OrderId = 1, DishId = dish.ID }
                ];

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _command.DeleteDish(dish.ID))
                .Should().ThrowAsync<InvlidDeleteDishException>();


            var unchangedDish = await _context.Dishes.FindAsync(dish.ID);
            unchangedDish.Should().NotBeNull();
            unchangedDish!.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteDish_WithMixedOrderItems_ThrowsInvlidDeleteDishException()
        {
            // ARRANGE
            var dish = CreateDishTest();
            dish.OrderItems =
                [
                    new() { Id = 1, StatusId = (int)OrderItemStatus.Closed,OrderId = 1, DishId = dish.ID },
                    new() { Id = 2, StatusId = (int)OrderItemStatus.Preparing,OrderId = 1, DishId = dish.ID },
                    new() { Id = 3, StatusId = (int)OrderItemStatus.Closed,OrderId = 1, DishId = dish.ID }
                ];
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _command.DeleteDish(dish.ID))
                .Should().ThrowAsync<InvlidDeleteDishException>();
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

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            // ACT
            await _command.DeleteDish(dish.ID);

            // ASSERT
            var deletedDish = await _context.Dishes.FindAsync(dish.ID);
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
                new() { Id = 1, StatusId = (int)activeStatus, OrderId = 1, DishId = dish.ID }
            ];

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            // ACT & ASSERT
            await FluentActions.Invoking(() => _command.DeleteDish(dish.ID))
                .Should().ThrowAsync<InvlidDeleteDishException>();
        }

        [Fact]
        public async Task UpdateDish_UpdatesOnlyModifiedEntity()
        {
            // ARRANGE
            var dish1 = CreateDishTest();
            var dish2 = CreateDishTest();
            dish2.Name = "Dish 2";

            _context.Dishes.AddRange(dish1, dish2);
            await _context.SaveChangesAsync();

            var updateRequest = new UpdateDishRequest { Name = "Updated Dish 1", Description = "New", Price = 15m, Category = 1, Image = "new.jpg", IsActive = true };

            // ACT
            await _command.UpdateDish(dish1, updateRequest);

            // ASSERT
            var updatedDish1 = await _context.Dishes.FindAsync(dish1.ID);
            var unchangedDish2 = await _context.Dishes.FindAsync(dish2.ID);

            updatedDish1!.Name.Should().Be("Updated Dish 1");
            unchangedDish2!.Name.Should().Be("Dish 2");
        }
    }
}
