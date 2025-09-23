namespace Application.DataTransfers.Response.Order
{
    public  class OrderUpdateResponse
    {
        public long OrderNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
