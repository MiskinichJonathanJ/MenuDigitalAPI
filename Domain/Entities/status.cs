namespace Domain.Entities
{
    public  class  Status
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Order> OrdersNav { get; set; } = [];
        public ICollection<OrderItem> OrderItemsNav { get; set; }  = [];
    }
}