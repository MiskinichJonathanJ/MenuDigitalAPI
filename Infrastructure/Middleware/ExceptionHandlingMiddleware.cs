using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            switch (ex)
            {
                case DishNameAlreadyExistsException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    break;
                case DishNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case InvalidDishPriceException or ArgumentException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case CategoryNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
