using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Dish
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public  bool IsAvailable { get; set; }
        public  string ImageURL { get; set; }
        public int CategoryId { get; set; }
        public Category CategoryNav { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}