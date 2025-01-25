using System;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    /// <summary>
    /// Set style width for grid data export excel. Use double for input parametter.
    /// </summary>
    [AttributeUsage(AttributeTargets.All | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class WidthAttribute : Attribute
    {
        private double _width;

        public WidthAttribute( double width)
        {
            this._width = width;
        }
    }
}