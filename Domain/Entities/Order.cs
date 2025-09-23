namespace Domain.Entities
{
    public class  Order
    {
        public long OrderId { get; set; }
        public required string DeliveryTo { get; set; }
        public  string? Notes { get; set; }
        public decimal Price { get; set; }

        public int OverallStatus { get; set; }
        public Status? StatusNav { get; set; }

        public int DeliveryType { get; set; }
        public DeliveryType? DeliveryTypeNav { get; set; }


        public ICollection<OrderItem> Items { get; set; } = [];
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}