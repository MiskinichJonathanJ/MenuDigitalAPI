using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Dish
    {
        public Guid DishId { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public  bool Available { get; set; }
        public required string ImageUrl { get; set; }
        public int Category { get; set; }
        public Category? CategoryNav { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = [];
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}