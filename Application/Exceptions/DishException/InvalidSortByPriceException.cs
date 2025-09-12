namespace Application.Exceptions.DishException
{
    public class InvalidSortByPriceException :  ValidationException
    {
        public InvalidSortByPriceException() : base("El valor de 'sortByPrice' es inválido. Debe ser 'asc' o 'desc'.")
        {
        }
    }
}
