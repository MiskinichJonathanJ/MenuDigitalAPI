namespace Application.Exceptions.OrderException
{
    public class OrderNotFoundException : NotFoundException { public OrderNotFoundException() : base("Orden no encontrada") { } }
}
