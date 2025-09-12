namespace Application.Exceptions.DishException
{
    public class InvalidDishPriceException : ValidationException 
    { 
        public InvalidDishPriceException() : base("El precio debe ser mayor a cero") { } 
    }
}
