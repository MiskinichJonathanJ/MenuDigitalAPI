using Microsoft.AspNetCore.Http;

namespace Application.Exceptions
{
    public class ValidationException : DomainException
    {
        protected ValidationException(string message) : base(message, StatusCodes.Status400BadRequest) { }
    }
}
