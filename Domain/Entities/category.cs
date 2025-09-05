namespace Domain.Entities
{
    public class Category
    {
        public  int  Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Order { get; set; }
        public ICollection<Dish> Dishes { get; set; } = [];
    }
}