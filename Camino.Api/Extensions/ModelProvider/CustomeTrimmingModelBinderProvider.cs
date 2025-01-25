using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Camino.Api.Extensions.ModelProvider
{
    public class CustomeTrimmingModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext objModelBinderProviderContext)
        {
            if (objModelBinderProviderContext == null)
            {
                throw new ArgumentNullException(nameof(objModelBinderProviderContext));
            }
            
            if (!objModelBinderProviderContext.Metadata.IsComplexType && objModelBinderProviderContext.Metadata.ModelType == typeof(string))
            {
                var loggerFactory = objModelBinderProviderContext.Services.GetRequiredService<ILoggerFactory>();
                return new CustomeModelBinder(new SimpleTypeModelBinder(objModelBinderProviderContext.Metadata.ModelType, loggerFactory));
            }
            return null;
        }
    }
}