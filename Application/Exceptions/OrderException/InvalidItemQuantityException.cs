namespace Application.Exceptions.OrderException
{
    public class InvalidItemQuantityException : OrderRequestException
    {
        public InvalidItemQuantityException() : base("La cantidad de un plato debe ser mayor a cero.") { }
    }
}
