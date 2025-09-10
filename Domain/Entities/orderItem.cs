namespace Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public  string?  Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        public required int StatusId { get; set; }
        public Status? Status { get; set; }

        public required int OrderId { get; set; }
        public Order? OrderNav { get; set; }

        public required Guid DishId { get; set; }
        public Dish? DishNav { get; set; }
    }
}