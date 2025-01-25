using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Camino.Api.Models.Error
{
    public class ApiError
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string Detail { get; set; }
        public List<ValidationError> Errors { get; set; }

        public ApiError(string message)
        {
            this.Message = message;
            IsError = true;
        }

        public ApiError(string message, string detail)
        {
            this.Message = message;
            this.Detail = detail;
            IsError = true;
        }
        public ApiError(ApiException apiException)
        {
            this.IsError = true;
            Message = apiException.Message;
            Errors = apiException.Errors;
        }
    }
}
