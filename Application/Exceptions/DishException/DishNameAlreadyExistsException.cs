namespace Application.Exceptions.DishException
{
    public class DishNameAlreadyExistsException : ConflictException { public DishNameAlreadyExistsException() : base("Ya existe un plato con ese nombre") { } }
}
