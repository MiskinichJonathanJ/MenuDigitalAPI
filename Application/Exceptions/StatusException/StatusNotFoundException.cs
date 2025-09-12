namespace Application.Exceptions.StatusException
{
    public class StatusNotFoundException : NotFoundException { public StatusNotFoundException() : base("Estado no encontrado") { } }
}
