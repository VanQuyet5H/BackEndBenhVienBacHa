using System;

namespace Camino.Core.Audit
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class AuditIgnoreAttribute : Attribute
    {
        public AuditIgnoreAttribute()
        {
           
        }
    }
}