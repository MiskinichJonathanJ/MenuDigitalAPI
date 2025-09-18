namespace Application.DataTransfers.Request.Order
{
    public class OrderRequest
    {
        public required ICollection<Items> Items { get; set; } = [];
        public required DeliveryRequest Delivery { get; set; }
        public string? Notes { get; set; }
    }
}
