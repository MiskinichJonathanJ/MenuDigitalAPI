namespace Application.Exceptions.DeliveryException
{
    public class DeliveryTypeNotFoundException : NotFoundException { public DeliveryTypeNotFoundException() : base("Tipo de entrega no encontrado") { } }
}
