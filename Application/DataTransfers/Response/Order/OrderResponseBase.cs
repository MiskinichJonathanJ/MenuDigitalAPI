namespace Application.DataTransfers.Response.Order
{
    public abstract class OrderResponseBase
    {
        public long OrderNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
