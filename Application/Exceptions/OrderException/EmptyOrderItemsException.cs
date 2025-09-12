namespace Application.Exceptions.OrderException
{
    public class EmptyOrderItemsException : OrderRequestException
    {
        public EmptyOrderItemsException() : base("El pedido debe contener al menos un plato.") { }
    }
}
