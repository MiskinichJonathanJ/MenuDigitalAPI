using Application.Interfaces.IOrder;
using Application.UseCase.OrderUse;
using Moq;

namespace UnitTest.Unit.UseCase.OrderTest
{
    public class OrderServiceTestBase
    {
        protected Mock<IOrderValidator> validator = new();
        protected Mock<IOrderCommand> command = new();
        protected Mock<IOrderMapper> mapper = new();
        protected Mock<IOrderQuery> query = new();
        protected OrderService service;

        protected OrderServiceTestBase()
        {
            command = new Mock<IOrderCommand>(MockBehavior.Loose);
            query = new Mock<IOrderQuery>(MockBehavior.Loose);
            mapper = new Mock<IOrderMapper>(MockBehavior.Loose);
            validator = new Mock<IOrderValidator>(MockBehavior.Loose);

            service = new OrderService(
                command.Object,
                validator.Object,
                mapper.Object,
                query.Object
            );
        }
    }
}
