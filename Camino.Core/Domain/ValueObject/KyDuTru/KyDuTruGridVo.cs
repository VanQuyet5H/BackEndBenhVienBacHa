using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.KyDuTru
{
    public class KyDuTruGridVo : GridItem
    {
        public string KyDuTru { get; set; }
        public DateTime TuNgay { get; set; }
        public string TuNgayDisplay => TuNgay.ApplyFormatDate();
        public DateTime DenNgay { get; set; }
        public string DenNgayDisplay => DenNgay.ApplyFormatDate();
        public long NhanVienTaoId { get; set; }
        public string NhanVienTaoDisplay { get; set; }
        public bool? MuaDuocPham { get; set; }
        public bool? MuaVatTu { get; set; }
        public string ApDung => MuaDuocPham == true && MuaVatTu == true ? "Dược phẩm & Vật tư" : (MuaDuocPham == true ? "Dược phẩm" : "Vật tư");
        public bool HieuLuc { get; set; }
        public string HieuLucDisplay => HieuLuc ? "Đang sử dụng" : "Ngừng sử dụng";
        public DateTime? NgayTao { get; set; }
        public string NgayTaoDisplay => NgayTao.Value.ApplyFormatDateTimeSACH();
        public DateTime NgayBatDauLap { get; set; }
        public string NgayBatDauLapDisplay => NgayBatDauLap.ApplyFormatDate();
        public DateTime NgayKetThucLap { get; set; }
        public string NgayKetThucLapDisplay => NgayKetThucLap.ApplyFormatDate();
    }
}
