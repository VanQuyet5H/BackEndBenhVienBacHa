using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTiepNhanBenhPhamGridVo : GridItem
    {
        public DateTime GioNhan { get; set; }
        public string GioNhanString => GioNhan.ToString("dd/MM/yyyy HH:mm");
        public string Barcode { get; set; }
        public int Tuoi { get; set; }
        public int? NamSinh { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string MauBenhPham { get; set; }
        public string NguoiGiaoMau { get; set; }
        public string NguoiNhanMau { get; set; }
        public string NguoiGiaoKQ { get; set; }
        public string NguoiNhanKQ { get; set; }
        public string GhiChu { get; set; }
    }
}