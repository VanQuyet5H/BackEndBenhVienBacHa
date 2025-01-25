using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT
{
    public class HoSoChiTietDVKTViewModel : BaseViewModel
    {
        public string MaLienKet { get; set; }
        public string MaDichVu { get; set; }
        public string MaVatTu { get; set; }
        public string GoiVatTuYTe { get; set; }
        public string TenVatTu { get; set; }
        public string ThongTinThau { get; set; }
        public int? PhamVi { get; set; }
        public double? MucThanhToanToiDa { get; set; }
        public int? MucHuong { get; set; }
        public double? TienNguonKhac { get; set; }
        public double? TienBenhNhanThanhToan { get; set; }
        public double? TienBaoHiemThanhToan { get; set; }
        public double? TienBenhNhanCungChiTra { get; set; }
        public double? TienNgoaiDanhSach { get; set; }
        public string MaGiuong { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi? MaNhom { get; set; }
        public string TenDichVu { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuong { get; set; }
        public double? DonGia { get; set; }
        public int? TyLeThanhToan { get; set; }
        public double? ThanhTien { get; set; }
        public string MaKhoa { get; set; }
        public string MaBacSi { get; set; }
        public string MaBenh { get; set; }
        public string NgayYLenh { get; set; }
        public DateTime?  NgayYLenhTime { get; set; }
        public string NgayKetQua { get; set; }
        public DateTime? NgayKetQuaTime { get; set; }
        public Enums.EnumMaPhuongThucThanhToan? MaPhuongThucThanhToan { get; set; }
    }
}
