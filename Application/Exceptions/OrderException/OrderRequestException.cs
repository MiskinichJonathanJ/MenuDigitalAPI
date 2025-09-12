namespace Application.Exceptions.OrderException
{
    public class OrderRequestException : ValidationException
    {
        protected OrderRequestException(string message) : base(message) { }
    }
}
