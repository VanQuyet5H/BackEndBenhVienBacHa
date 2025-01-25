using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KhamDoanCongTyGridVo : GridItem
    {
        public string MaCongTy { get; set; }

        public string TenCongTy { get; set; }

        public string LoaiCongTy { get; set; }

        public string DiaChi { get; set; }
        
        public string DienThoai { get; set; }

        public string MaSoThue { get; set; }

        public string TaiKhoanNganHang { get; set; }

        public string DaiDien { get; set; }

        public string NguoiLienHe { get; set; }

        public bool CoHoatDong { get; set; }

        public string TrangThai => CoHoatDong ? "Hoạt động" : "Tạm ngưng";
    }

    public class KhamDoanCongTyTimKiemNangCaoVo
    {
        public string SearchString { get; set; }
    }
}
