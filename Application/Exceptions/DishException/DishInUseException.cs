namespace Application.Exceptions.DishException
{
    public class DishInUseException : ConflictException { public DishInUseException() : base("No se puede eliminar: plato en uso") { } }
}
