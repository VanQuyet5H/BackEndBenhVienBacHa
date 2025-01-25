using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT
{
    public class ThongTinBenhNhanViewModel
    {
        ThongTinBenhNhanViewModel()
        {
            HoSoChiTietThuoc = new List<HoSoChiTietThuocViewModel>();
            HoSoChiTietDienBienBenh = new List<HoSoChiTietDienBienBenhViewModel>();
            HoSoCanLamSang = new List<HoSoCanLamSangViewModel>();
            HoSoChiTietDVKT = new List<HoSoChiTietDVKTViewModel>();
        }
        public string MaLienKet { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string MaThe { get; set; }
        public string MaCoSoKCBBanDau { get; set; }
        public string GiaTriTheTu { get; set; }
        public string GiaTriTheDen { get; set; }
        public string TenBenh { get; set; }
        public string MaBenh { get; set; }
        public string MienCungChiTra { get; set; }
        public string MaBenhKhac { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public string MaNoiChuyen { get; set; }
        public Enums.EnumMaTaiNan? MaTaiNan { get; set; }
        public string NgayVao { get; set; }
        public DateTime? NgayVaoTime { get; set; }
        public string NgayRa { get; set; }
        public DateTime? NgayRaTime { get; set; }
        public int? SoNgayDieuTri { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }
        public string NgayThanhToan { get; set; }
        public DateTime? NgayThanhToanTime { get; set; }
        public double? TienThuoc { get; set; }
        public double? TienVatTuYTe { get; set; }
        public double? TienTongChi { get; set; }
        public double? TienBenhNhanThanhToan { get; set; }
        public double? TienBaoHiemThanhToan { get; set; }
        public double? TienNguonKhac { get; set; }
        public double? TienNgoaiDanhSach { get; set; }
        public double? TienBenhNhanCungChiTra { get; set; }
        public int? NamQuyetToan { get; set; }
        public int? ThangQuyetToan { get; set; }
        public Enums.EnumMaHoaHinhThucKCB? MaLoaiKCB { get; set; }
        public string MaKhoa { get; set; }
        public string MaCSKCB { get; set; }
        public string MaKhuVuc { get; set; }
        public string MaPhauThuatQuocTe { get; set; }
        public double? CanNang { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string DataJson { get; set; }
        public bool? IsDownLoad { get; set; }


        public List<HoSoChiTietThuocViewModel> HoSoChiTietThuoc { get; set; }
        public List<HoSoChiTietDienBienBenhViewModel> HoSoChiTietDienBienBenh { get; set; }
        public List<HoSoCanLamSangViewModel> HoSoCanLamSang { get; set; }
        public List<HoSoChiTietDVKTViewModel> HoSoChiTietDVKT { get; set; }

    }
    public class ThongTinBenhNhanViewModels
    {
        ThongTinBenhNhanViewModels()
        {
            ThongTinBenhNhanVMs = new List<ThongTinBenhNhanViewModel>();
        }

        public List<ThongTinBenhNhanViewModel> ThongTinBenhNhanVMs { get; set; }
    }    
}

