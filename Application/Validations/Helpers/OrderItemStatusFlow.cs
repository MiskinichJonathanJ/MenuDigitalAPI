namespace Application.Validations.Helpers
{
    public class OrderItemStatusFlow
    {
        public enum OrderItemStatus
        {
            Pending = 1,
            Preparing = 2,
            Ready = 3,
            Delivered = 4,
            Closed = 5
        }

        private static readonly Dictionary<OrderItemStatus, OrderItemStatus[]> Allowed =
        new()
        {
            { OrderItemStatus.Pending,   new[]{ OrderItemStatus.Preparing } },
            { OrderItemStatus.Preparing, new[]{ OrderItemStatus.Ready } },
            { OrderItemStatus.Ready,     new[]{ OrderItemStatus.Delivered } },
            { OrderItemStatus.Delivered, new[]{ OrderItemStatus.Closed } },
            { OrderItemStatus.Closed,    Array.Empty<OrderItemStatus>() }
        };

        public static bool CanTransition(OrderItemStatus current, OrderItemStatus next) =>
            Allowed[current].Contains(next);
    }
}
