namespace Application.DataTransfers.Request.Order
{
    public class OrderUpdateRequest
    {
        public ICollection<Items> Items { get; set; } = [];
    }
}
