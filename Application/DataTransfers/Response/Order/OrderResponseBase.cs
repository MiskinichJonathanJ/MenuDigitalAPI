namespace Application.DataTransfers.Response.Order
{
    public abstract class OrderResponseBase
    {
        public int OrderNumber { get; set; }
        public double TotalMount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
