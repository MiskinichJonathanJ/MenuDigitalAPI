namespace Application.Exceptions.OrderException
{
    public class InvalidDateOrderException : OrderRequestException
    {
        public InvalidDateOrderException() : base("El rango de fechar es invalido") { }
    }
}
