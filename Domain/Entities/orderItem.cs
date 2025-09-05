namespace Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public  string?  Notes { get; set; }
        public DateTime CreatedDate { get; set; }

        public  int StatusId { get; set; }
        public Status? Status { get; set; }

        public int OrderId { get; set; }
        public Order? OrderNav { get; set; }

        public Guid DishId { get; set; }
        public Dish? DishNav { get; set; }
    }
}