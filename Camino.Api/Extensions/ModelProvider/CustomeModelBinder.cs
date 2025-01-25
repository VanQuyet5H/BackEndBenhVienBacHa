using System;
using System.Threading.Tasks;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Camino.Api.Extensions.ModelProvider
{
    public class CustomeModelBinder : IModelBinder
    {
        private readonly IModelBinder _objIModelBinder;
        public CustomeModelBinder(IModelBinder objIModelBinder)
        {
            _objIModelBinder = objIModelBinder ?? throw new
                                   ArgumentNullException(nameof(objIModelBinder));
        }
        public Task BindModelAsync(ModelBindingContext objModelBindingContext)
        {
            if (objModelBindingContext == null)
            {
                throw new ArgumentNullException(nameof(objModelBindingContext));
            }
            var valueProviderResult =
                objModelBindingContext.ValueProvider.GetValue(objModelBindingContext.ModelName);
            if (valueProviderResult != null && valueProviderResult.FirstValue is string str &&
                !string.IsNullOrEmpty(str))
            {
                objModelBindingContext.Result = ModelBindingResult.Success(str.Trim().ConvertUnicodeString());
                return Task.CompletedTask;
            }
            return _objIModelBinder.BindModelAsync(objModelBindingContext);
        }
    }
}