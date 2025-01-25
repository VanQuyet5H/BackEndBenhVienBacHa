using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class LayMauXetNghiemInBarcodeVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public string BarcodeId { get; set; }
        public int BarcodeNumber { get; set; }
        public int SoLuong { get; set; }
        public string HostingName { get; set; }
        public string TenBenhNhan { get; set; }
        public string BarcodeByBarcodeId { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhFotmat => GioiTinh.GetDescription();
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        //public string NgaySinhFotmat => (NgaySinh != null ? NgaySinh.ToString() : "0") + "/" +
        //                                (ThangSinh != null ? ThangSinh.ToString() : "0") + "/" +
        //                                (NamSinh != null ? NamSinh.ToString() : "0");
        public string NgaySinhFotmat => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string GioCapCode { get; set; }
    }
}
