namespace Application.Exceptions.OrderException
{
    public class OrderItemNotFoundException  : NotFoundException { public OrderItemNotFoundException() : base("Item no encontrado en la orden") {} }
}
