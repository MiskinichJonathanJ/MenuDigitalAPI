namespace Domain.Entities
{
	public class DeliveryType
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public  ICollection<Order> OrdersNav { get; set; } = [];
    }
}