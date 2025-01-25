using System;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhGrid : GridItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Enums.DataType DataType { get; set; }
        public Enums.LoaiCauHinh LoaiCauHinh { get; set; }

        public bool IsCauHinh { get; set; }

        public string TenLoaiCauHinh
        {
            get
            {
                var temp = Name.Substring(0, Name.IndexOf("."));
                var tenLoaiCauHinh = Helpers.EnumHelper
                    .GetListEnum<Enums.LoaiCauHinh>()
                    //.Select(s => new LookupItemVo()
                    //{
                    //    DisplayName = Enum.GetName(typeof(Enums.LoaiCauHinh), (int)s)
                    //})
                    .Where(s => Enum.GetName(typeof(Enums.LoaiCauHinh), (int)s) == temp)
                    .Select(s => s.GetDescription()).FirstOrDefault();


                return tenLoaiCauHinh;
            }
        }
    }
}
