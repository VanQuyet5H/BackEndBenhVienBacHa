using System;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    /// <summary>
    /// Set column name for data export excel. Use string title: "example".
    /// </summary>
    [AttributeUsage(AttributeTargets.All | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TitleGridChildAttribute : Attribute
    {
        private string _title;

        public TitleGridChildAttribute(string title)
        {
            this._title = title;
        }
    }
}