namespace Domain.Entities
{
    public class OrderItem
    {
        public long OrderItemId { get; set; }
        public int Quantity { get; set; }
        public  string?  Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        public required int Status { get; set; }
        public Status? StatusNav { get; set; }

        public required long Order { get; set; }
        public Order? OrderNav { get; set; }

        public required Guid Dish { get; set; }
        public Dish? DishNav { get; set; }
    }
}