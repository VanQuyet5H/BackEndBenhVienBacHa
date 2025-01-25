using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BHYT
{
    public class ThongTinBenhNhan
    {
        public ThongTinBenhNhan()
        {
            HoSoChiTietThuoc = new List<HoSoChiTietThuoc>();
            HoSoChiTietDienBienBenh = new List<HoSoChiTietDienBienBenh>();
            HoSoCanLamSang = new List<HoSoCanLamSang>();
            HoSoChiTietDVKT = new List<HoSoChiTietDVKT>();
        }
        public string MaLienKet { get; set; }
        public int? STT { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }//old:public string NgaySinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }//old:public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string MaThe { get; set; }
        public string MaCoSoKCBBanDau { get; set; }
        public DateTime GiaTriTheTu { get; set; }//old:public string GiaTriTheTu { get; set; }
        public DateTime? GiaTriTheDen { get; set; }//old:public string GiaTriTheDen { get; set; }
        public DateTime? MienCungChiTra { get; set; }//old:public string MienCungChiTra { get; set; }
        public string TenBenh { get; set; }
        public string MaBenh { get; set; }
        public string MaBenhKhac { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public string MaNoiChuyen { get; set; }
        public Enums.EnumMaTaiNan? MaTaiNan { get; set; }
        public DateTime NgayVao { get; set; }      //old:public DateTime? NgayVaoTime { get; set; }
                                                   //old:public string NgayVao => NgayVaoTime?.ApplyFormatDate();
                                                   //old:public string NgayVaoStr { get; set; }
        public DateTime NgayRa { get; set; } //old:public DateTime? NgayRaTime { get; set; }
                                             //old:public string NgayRa => NgayRaTime?.ApplyFormatDate();
                                             //old:public string NgayRaStr { get; set; }
        public int SoNgayDieuTri { get; set; }//old:public double SoNgayDieuTri { get; set; }
        public Enums.EnumKetQuaDieuTri KetQuaDieuTri { get; set; }//old:public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public Enums.EnumTinhTrangRaVien TinhTrangRaVien { get; set; }//old:public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }
        public DateTime? NgayThanhToan { get; set; }//public DateTime? NgayThanhToanTime { get; set; }
                                                    //public string NgayThanhToan => NgayThanhToanTime?.ApplyFormatDate();
        public decimal? TienThuoc => HoSoChiTietThuoc.Sum(o => o.ThanhTien);//old:public double? TienThuoc { get; set; }
        public decimal? TienVatTuYTe => HoSoChiTietDVKT.Where(o=>o.MaNhom == Enums.EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT || o.MaNhom == Enums.EnumDanhMucNhomTheoChiPhi.VTYTThanhToanTheoTyLe).Sum(o => o.ThanhTien);//old:public double? TienVatTuYTe { get; set; }
        public decimal TienTongChi => HoSoChiTietThuoc.Sum(o => o.ThanhTien) + HoSoChiTietDVKT.Sum(o => o.ThanhTien);//old:public double? TienTongChi { get; set; }
        public decimal? TienBenhNhanTuTra => HoSoChiTietThuoc.Select(o => o.TienBenhNhanTuTra).DefaultIfEmpty(0).Sum() + HoSoChiTietDVKT.Select(o => o.TienBenhNhanTuTra).DefaultIfEmpty(0).Sum();//old:public double? TienBenhNhanThanhToan { get; set; }
        public decimal? TienBenhNhanCungChiTra => HoSoChiTietThuoc.Select(o => o.TienBenhNhanCungChiTra).DefaultIfEmpty(0).Sum() + HoSoChiTietDVKT.Select(o => o.TienBenhNhanCungChiTra).DefaultIfEmpty(0).Sum();//old:public double? TienBenhNhanCungChiTra { get; set; }
        public decimal TienBaoHiemThanhToan => HoSoChiTietThuoc.Sum(o => o.TienBaoHiemThanhToan) + HoSoChiTietDVKT.Sum(o => o.TienBaoHiemThanhToan);//old:public double? TienBaoHiemThanhToan { get; set; }
        public decimal? TienNguonKhac => HoSoChiTietThuoc.Select(o => o.TienNguonKhac).DefaultIfEmpty(0).Sum() + HoSoChiTietDVKT.Select(o => o.TienNguonKhac).DefaultIfEmpty(0).Sum();//old:public double? TienNguonKhac { get; set; }
        public decimal? TienNgoaiDinhSuat => HoSoChiTietThuoc.Select(o => o.TienNgoaiDinhSuat).DefaultIfEmpty(0).Sum() + HoSoChiTietDVKT.Select(o => o.TienNgoaiDinhSuat).DefaultIfEmpty(0).Sum();//old:public double? TienNgoaiDanhSach { get; set; }
        public int NamQuyetToan { get; set; }//old:public int? NamQuyetToan { get; set; }
        public int ThangQuyetToan { get; set; }//old:public int? ThangQuyetToan { get; set; }
        public Enums.EnumMaHoaHinhThucKCB MaLoaiKCB { get; set; }//old:public Enums.EnumMaHoaHinhThucKCB? MaLoaiKCB { get; set; }
        public string MaKhoa { get; set; }
        public string MaCSKCB { get; set; }
        public string MaKhuVuc { get; set; }
        public string MaPhauThuatQuocTe { get; set; }
        public double? CanNang { get; set; }
        public string DataJson { get; set; }
        public int? MucHuong { get; set; }
        public bool? IsDownLoad { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }//update 08/04/2022
        public long YeuCauTiepNhanId { get; set; }//new
        public int Version { get; set; }
        public  bool IsVersion { get; set; }

        public List<HoSoChiTietThuoc> HoSoChiTietThuoc { get; set; }
        public List<HoSoChiTietDienBienBenh> HoSoChiTietDienBienBenh { get; set; }
        public List<HoSoCanLamSang> HoSoCanLamSang { get; set; }
        public List<HoSoChiTietDVKT> HoSoChiTietDVKT { get; set; }
    }
}
