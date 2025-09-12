using Microsoft.AspNetCore.Http;

namespace Application.Exceptions
{
    public  abstract class DomainException : Exception
    {
        public int StatusCode { get; }
        protected DomainException(string message, int statusCode = StatusCodes.Status400BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
