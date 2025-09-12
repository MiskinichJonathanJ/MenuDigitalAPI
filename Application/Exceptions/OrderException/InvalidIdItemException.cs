namespace Application.Exceptions.OrderException
{
    public class InvalidIdItemException : OrderRequestException
    {
        public InvalidIdItemException() : base("El id de un plato es inválido.") { }
    }
}
