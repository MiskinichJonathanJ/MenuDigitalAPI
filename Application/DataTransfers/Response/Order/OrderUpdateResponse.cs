namespace Application.DataTransfers.Response.Order
{
    public  class OrderUpdateResponse
    {
        public long OrderNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
