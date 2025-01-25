using Microsoft.Extensions.DependencyInjection;

namespace Camino.Core.DependencyInjection.Attributes
{
    public class TransientDependencyAttribute : DependencyAttribute
    {
        public TransientDependencyAttribute() : base(ServiceLifetime.Transient)
        {
        }
    }
}