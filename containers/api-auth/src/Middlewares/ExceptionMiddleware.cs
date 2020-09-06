using Microsoft.AspNetCore.Http;
using Mist.Auth.Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Auth.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var statusCode = (int)HttpStatusCode.InternalServerError;
                var errorMessage = "An unexpected error has occurred.";

                if (ex is DomainException)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = ex.Message;
                }

                var body = new
                {
                    Error = errorMessage
                };

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = statusCode;
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(body)).ConfigureAwait(false);
            }
        }
    }
}
