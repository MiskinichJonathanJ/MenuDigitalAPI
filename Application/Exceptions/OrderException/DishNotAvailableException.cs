namespace Application.Exceptions.OrderException
{
    public class DishNotAvailableException : OrderRequestException
    {
        public DishNotAvailableException() : base("Uno o más platos no están disponibles en este momento.") { }
    }
}
