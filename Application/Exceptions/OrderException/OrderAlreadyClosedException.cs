namespace Application.Exceptions.OrderException
{
    public class OrderAlreadyClosedException : ConflictException
    {
        public OrderAlreadyClosedException() : base("No se puede modificar un pedido que ya está cerrado.") { }
    }
}
