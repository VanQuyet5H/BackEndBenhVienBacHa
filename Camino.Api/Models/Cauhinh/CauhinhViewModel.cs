using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.Cauhinh
{
    public class CauhinhViewModel : BaseViewModel
    {
        public CauhinhViewModel()
        {
            CauHinhTheoThoiGianChiTiets = new List<CauHinhTheoThoiGianChiTietViewModel>();
            CauHinhDanhSachChiTiets = new List<CauHinhDanhSachChiTietViewModel>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public int LoaiCauHinh { get; set; }

        public int DataType { get; set; }

        public List<CauHinhTheoThoiGianChiTietViewModel> CauHinhTheoThoiGianChiTiets { get; set; }
        public List<CauHinhDanhSachChiTietViewModel> CauHinhDanhSachChiTiets { get; set; }

        public Enums.LoaiCauHinh? DataTypeLoaiCauHinh
        {
            get
            {
                Enums.LoaiCauHinh? dataTypeLoaiCauHinh = null;
                if (!string.IsNullOrEmpty(Name))
                {
                    var temp = Name.Substring(0, Name.IndexOf("."));
                    dataTypeLoaiCauHinh = Core.Helpers.EnumHelper
                        .GetListEnum<Enums.LoaiCauHinh>()
                        .Where(s => Enum.GetName(typeof(Enums.LoaiCauHinh), (int) s) == temp)
                        .Select(s => s).FirstOrDefault();
                }

                return dataTypeLoaiCauHinh;
            }
        }
    }
}
