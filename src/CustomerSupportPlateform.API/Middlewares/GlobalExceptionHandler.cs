using CustomerSupportPlateform.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CustomerSupportPlateform.API.Middlewares;


public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        //int statusCode;string exceptionMessage;
        if(exception != null)
        {
            logger.LogError("An Unhandled exception occured");
            (int status, string message) = exception switch
            {
                ArgumentNullException => ((int)HttpStatusCode.BadRequest, exception.Message),
                ValidationException => ((int)HttpStatusCode.BadRequest, exception.Message),
                NotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
                _ => ((int)HttpStatusCode.InternalServerError, $"Something went wrong happen:{exception.Message}")

            };

            var problem = new ProblemDetails()
            {
                Detail = message,
                Status = status,
                Type = exception?.HelpLink,
            };

        
            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
        else
        {
            return true;
        }
        

        
    }
}