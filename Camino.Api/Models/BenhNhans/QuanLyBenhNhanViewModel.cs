using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BenhNhans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BenhNhans
{
    public class QuanLyBenhNhanViewModel : BaseViewModel
    {
        public QuanLyBenhNhanViewModel()
        {
            BenhNhanTienSuBenhs = new List<BenhNhanTienSuBenhsViewModel>();
            BenhNhanDiUngThuocs = new List<BenhNhanDiUngThuocsViewModel>();
            BenhNhanCongTyBaoHiemTuNhans = new List<BenhNhanBaoHiemTuNhansViewModel>();
        }
        public bool? CoBHYT { get; set; }
        public bool? CoBHTN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }
        public string NguoiLienHeDiaChi { get; set; }
        public long? NguoiLienHePhuongXaId { get; set; }
        public long? NguoiLienHeQuanHuyenId { get; set; }
        public long? NguoiLienHeTinhThanhId { get; set; }
        public string BHYTMaSoThe { get; set; }
        
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTDiaChi { get; set; }

        public string BHYTCoQuanBHXH { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public string BHYTMaDKBD { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }


        public string NhomMauText { get; set; }
        public string NgheNghiepText { get; set; }
        public string QuocTichText { get; set; }
        public string NguoiLienHeQuanHeNhanThanText { get; set; }


        public List<BenhNhanTienSuBenhsViewModel> BenhNhanTienSuBenhs { get; set; }
        public List<BenhNhanDiUngThuocsViewModel> BenhNhanDiUngThuocs { get; set; }
        public List<BenhNhanBaoHiemTuNhansViewModel> BenhNhanCongTyBaoHiemTuNhans { get; set; }


    }

    public class BenhNhanTienSuBenhsViewModel : BaseViewModel
    {
        public BenhNhanTienSuBenhsViewModel()
        {
            TenTienSuBenhs = new List<BenhNhanTienSuBenhChiTiet>();
        }
        public long? BenhNhanId { get; set; }
        public string TenBenh { get; set; }
        public Enums.EnumLoaiTienSuBenh? LoaiTienSuBenh { get; set; }
        public string TenLoaiTienSuBenh { get; set; }
        public long? BenhNhanTienSuBenhId { get; set; }
        public List<BenhNhanTienSuBenhChiTiet> TenTienSuBenhs { get; set; }
    }

   

    public class BenhNhanDiUngThuocsViewModel : BaseViewModel
    {
        public BenhNhanDiUngThuocsViewModel()
        {
            TenDiUngThuocs = new List<BenhNhanDiUngThuocChiTiet>();
        }
        public long? BenhNhanDiUngId { get; set; }
        public long? BenhNhanId { get; set; }
        public string BieuHienDiUng { get; set; }
        public string TenDiUng { get; set; }
        public Enums.LoaiDiUng? LoaiDiUng { get; set; }  
        public string LoaiDiUngDisplay { get; set; }
        public long? ThuocId { get; set; }
        public string TenThuoc { get; set; }
        public List<BenhNhanDiUngThuocChiTiet> TenDiUngThuocs { get; set; }
        public Enums.EnumMucDoDiUng? MucDo { get; set; }
        public string MucDoDisplay { get; set; }
    }

    public class BenhNhanBaoHiemTuNhansViewModel : BaseViewModel
    {
        public BenhNhanBaoHiemTuNhansViewModel()
        {
            CongTyBHTNIds = new List<long>();
        }
        public long? BenhNhanId { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string CongTyDisplay { get; set; }
        public string MaSoThe { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string NgayHieuLucDisplay { get; set; }
        public string NgayHetHanDisplay { get; set; }
        public List<long> CongTyBHTNIds { get; set; }
        public virtual BenhNhanCongTyBaoHiemTuNhanViewModel CongTyBaoHiemTuNhan { get; set; }
    }

    public class BenhNhanCongTyBaoHiemTuNhanViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public int? HinhThucBaoHiem { get; set; }
        public int? PhamViBaoHiem { get; set; }
        public string GhiChu { get; set; }
    }


}
