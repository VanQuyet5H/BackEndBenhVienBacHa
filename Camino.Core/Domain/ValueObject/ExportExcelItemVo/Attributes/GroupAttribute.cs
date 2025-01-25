using System;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    /// <summary>
    /// Set group for grid data export excel.
    /// </summary>
    [AttributeUsage(AttributeTargets.All | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class GroupAttribute : Attribute
    {
        public GroupAttribute()
        {
        }
    }
}