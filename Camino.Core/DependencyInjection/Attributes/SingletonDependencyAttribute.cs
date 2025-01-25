using Microsoft.Extensions.DependencyInjection;

namespace Camino.Core.DependencyInjection.Attributes
{
    public class SingletonDependencyAttribute : DependencyAttribute
    {
        public SingletonDependencyAttribute() : base(ServiceLifetime.Singleton)
        {
        }
    }
}