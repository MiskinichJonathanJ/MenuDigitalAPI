namespace Application.DataTransfers.Response.Order
{
    public  class OrderUpdateResponse
    {
        public int OrderNumber { get; set; }
        public double TotalMount { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
