using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT
{
    public class HoSoChiTietThuocViewModel 
    {
        public string MaLienKet { get; set; }
        public string MaThuoc { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi? MaNhom { get; set; }
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public string HamLuong { get; set; }
        public string DuongDung { get; set; }
        public string LieuDung { get; set; }
        public string SoDangKy { get; set; }
        public double? SoLuong { get; set; }
        public double? DonGia { get; set; }
        public int? TyLeThanhToan { get; set; }
        public double? ThanhTien { get; set; }
        public string MaKhoa { get; set; }
        public string MaBacSi { get; set; }
        public string MaBenh { get; set; }
        public string NgayYLenh { get; set; }
        public DateTime? NgayYLenhTime { get; set; }
        public string ThongTinThau { get; set; }
        public int? PhamVi { get; set; }
        public int? MucHuong { get; set; }
        public double? TienBenhNhanTuTra { get; set; }
        public double? TienNguonKhac { get; set; }
        public double? TienBaoHiemTuTra { get; set; }
        public double? TienBenhNhanTuChiTra { get; set; }
        public double? TienNgoaiDanhSach { get; set; }
        public Enums.EnumMaPhuongThucThanhToan? MaPhuongThucThanhToan { get; set; }
    }
}
