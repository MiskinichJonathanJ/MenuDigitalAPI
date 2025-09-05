namespace Domain.Entities
{
	public class DeliveryType
	{
		public int ID { get; set; }
		public required string Name { get; set; }
		public  ICollection<Order> OrdersNav { get; set; } = [];
    }
}