using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Camino.Api.Models.Error;
using Camino.Core.Helpers;
using Camino.Core.Infrastructure;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.CustomMiddleware
{
    public class CustomExceptionMiddleware
    {
        private const string JsonContentType = "application/json";
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        private readonly IHostingEnvironment _env;
        private readonly ILocalizationService _localizationService;

        public CustomExceptionMiddleware(RequestDelegate next, ILoggerManager logger, IHostingEnvironment env, ILocalizationService localizationService)
        {
            _logger = logger;
            _next = next;
            _env = env;
            _localizationService = localizationService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                TryLogRequest(httpContext.Request);
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex, _env, _localizationService);
            }
        }

        private void TryLogRequest(HttpRequest request)
        {            
            try
            {
                if (request.Path.HasValue && request.Path.Value.Contains("UpdatePhieuDieuTriThamKham"))
                {
                    _logger.LogTrace("TryLogRequest UpdatePhieuDieuTriThamKham");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TryLogRequest: {ex}");
            }
        }

        private void TryLogRequestBody(HttpRequest request, Encoding encoding = null)
        {
            bool hasRead = false;
            try
            {
                if (request.ContentType.ToLower() == JsonContentType && request.Path.HasValue && request.Path.Value.Contains("UpdatePhieuDieuTriThamKham"))
                {
                    _logger.LogTrace("TryLogRequest UpdatePhieuDieuTriThamKham");
                    if (encoding == null) encoding = new UTF8Encoding();
                    request.EnableBuffering();
                    var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                    if (buffer.Length > 0)
                    {
                        request.Body.Read(buffer, 0, buffer.Length);
                        hasRead = true;
                        var content = encoding.GetString(buffer);
                        _logger.LogTrace("TryLogRequest UpdatePhieuDieuTriThamKham", content);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"TryLogRequest: {ex}");
            }
            finally
            {
                if (hasRead)
                {
                    request.Body.Position = 0;
                }
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, IHostingEnvironment env, ILocalizationService localizationService)
        {
            context.Response.ContentType = JsonContentType;
            if (exception is ApiException ex)
            {
                // handle explicit 'known' API errors
                //context.Exception = null;
                context.Response.StatusCode = ex.StatusCode;
                string jsonString = JsonConvert.SerializeObject(new ApiError(ex));
                return context.Response.WriteAsync(jsonString);
            }
            else if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                string jsonString = JsonConvert.SerializeObject(new ApiError(localizationService.GetResource("ApiError.Unauthorized")));
                return context.Response.WriteAsync(jsonString);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var apiError = TranslateException(exception, localizationService);
                if (env.IsDevelopment())
                {
                    string jsonString = JsonConvert.SerializeObject(apiError ?? new ApiError(exception.GetBaseException().Message, exception.StackTrace));
                    return context.Response.WriteAsync(jsonString);
                }
                else
                {
                    string jsonString = JsonConvert.SerializeObject(apiError ?? new ApiError(localizationService.GetResource("ApiError.UnknownError")));
                    return context.Response.WriteAsync(jsonString);
                }
            }
        }

        private static ApiError TranslateException(Exception exception, ILocalizationService localizationService)
        {
            if (exception is DbUpdateConcurrencyException)
            {
                return new ApiError(localizationService.GetResource("ApiError.ConcurrencyError"));
            }
            if (exception is DbUpdateException)
            {
                if (exception.InnerException != null && exception.InnerException.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    return new ApiError(localizationService.GetResource("ApiError.DeleteConflictedError"));
                }
            }
            if (!string.IsNullOrEmpty(exception.Message) && exception.Message.Contains("Value cannot be null.") && exception.Message.Contains("Parameter name: entity"))
            {
                return new ApiError(localizationService.GetResource("ApiError.EntityNull"));
            }
            if (exception.GetBaseException().Source == "Camino.Services")
            {
                return new ApiError(exception.GetBaseException().Message, exception.StackTrace);
            }
            
            return null;
        }
    }
}
