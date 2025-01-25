using System;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    [AttributeUsage(AttributeTargets.All | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class HeightWidthAttribute : Attribute
    {
        private double _height;
        private double _width;

        public HeightWidthAttribute(double height, double width)
        {
            this._height = height;
            this._width = width;
        }
    }
}