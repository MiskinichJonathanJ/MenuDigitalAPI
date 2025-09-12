namespace Application.Exceptions.OrderException
{
    public class InvalidOrderStatusTransitionException : ValidationException
    {
        public InvalidOrderStatusTransitionException() : base("La transición de estado del pedido no es válida.") { }
    }
}
