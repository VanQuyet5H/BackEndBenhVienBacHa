using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Camino.Api.Models.Error
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public List<ValidationError> Errors { get; set; }

        public ApiException(string message, int statusCode = 500, List<ValidationError> errors = null) :
            base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
    public class ValidationApiException : ApiException
    {
        public ValidationApiException(ModelStateDictionary modelState) :
            base("Validation Failed", (int) HttpStatusCode.BadRequest)
        {
            Errors = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
        }
    }
}
