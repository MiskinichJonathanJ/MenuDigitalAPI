using Application.DataTransfers.Response.OrderItem;

namespace Application.DataTransfers.Response.Order
{
    public class OrderDetailsResponse : OrderResponseBase
    {
        public string? DeliveryTo { get; set; }
        public string? Notes { get; set; }
        public GenericResponse? Status { get; set; }
        public  GenericResponse? DeliveryType { get; set; }
        public List<OrderItemResponse>? Items { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
