using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungBenhNhanTienSuBenhViewModel : BaseViewModel
    {
        public long BenhNhanId { get; set; }
        public string TenBenh { get; set; }
        public Enums.EnumLoaiTienSuBenh? LoaiTienSuBenh { get; set; }
        public string TenLoaiTienSuBenh
        {
            get { return LoaiTienSuBenh != null ? LoaiTienSuBenh.GetDescription() : null; }
        }
        public TiemChungBenhNhanViewModel BenhNhan { get; set; }
    }
}
