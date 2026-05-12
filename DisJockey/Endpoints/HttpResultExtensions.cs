using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DisJockey.Endpoints;

public static class HttpResultExtensions
{
    extension(List<Error> errors)
    {
        public IResult Problem()
        {
            //if (errors.All(error => error.Type == ErrorType.Validation))
            //{
            //    return ValidationProblem(errors);
            //}

            return Problem(errors[0]);
        }
    }

    extension(Error error)
    {
        public IResult Problem()
        {
            var statusCode = error.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError,
            };

            return Results.Problem(statusCode: statusCode, title: error.Description);
        }
    }
}
