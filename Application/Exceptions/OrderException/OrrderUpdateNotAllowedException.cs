namespace Application.Exceptions.OrderException
{
    public class OrrderUpdateNotAllowedException : ValidationException
    {
        public OrrderUpdateNotAllowedException() : base("No se puede actualizar el pedido en su estado actual.") { }
    }
}
