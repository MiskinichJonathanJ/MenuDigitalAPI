namespace Domain.Entities
{
    public class  Order
    {
        public int Id { get; set; }
        public required string DeliveryTo { get; set; }
        public  string? Notes { get; set; }
        public decimal Price { get; set; }

        public int OverallStatusID { get; set; }
        public Status? StatusNav { get; set; }

        public int DeliveryTypeID { get; set; }
        public DeliveryType? DeliveryTypeNav { get; set; }


        public ICollection<OrderItem> Items { get; set; }   = [];
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}