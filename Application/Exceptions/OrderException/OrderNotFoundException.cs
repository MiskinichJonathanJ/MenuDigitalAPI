namespace Application.Exceptions.OrderException
{
    public class StatusNotFoundException : NotFoundException { public StatusNotFoundException() : base("Estado no encontrado") { } }
}
