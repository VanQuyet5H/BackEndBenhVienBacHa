using Microsoft.Extensions.DependencyInjection;

namespace Camino.Core.DependencyInjection.Attributes
{
    public class ScopedDependencyAttribute : DependencyAttribute
    {
        public ScopedDependencyAttribute() : base(ServiceLifetime.Scoped)
        {
        }
    }
}