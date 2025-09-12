using Microsoft.AspNetCore.Http;

namespace Application.Exceptions
{
    public class ConflictException : DomainException
    {
        protected ConflictException(string message) : base(message, StatusCodes.Status409Conflict) { }
    }
}
