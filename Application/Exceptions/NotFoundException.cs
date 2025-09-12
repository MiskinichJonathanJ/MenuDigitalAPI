using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class NotFoundException: DomainException
    {
        protected NotFoundException(string message) : base(message, StatusCodes.Status404NotFound) { }
    }
}
