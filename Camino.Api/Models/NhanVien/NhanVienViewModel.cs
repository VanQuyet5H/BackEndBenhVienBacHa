using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using Camino.Api.Models.KhoaPhongNhanVien;
namespace Camino.Api.Models.NhanVien
{
    public class NhanVienViewModel : BaseViewModel
    {
        public NhanVienViewModel()
        {
            KhoaPhongNhanViens = new List<KhoaPhongNhanVienViewModel>();
            KhoNhanVienQuanLys = new List<KhoNhanVienQuanLyModel>();
            LstRole = new List<long>();
            ChucVuIds = new List<long>();
            HoSoNhanVienFileDinhKems = new List<HoSoNhanVienFileDinhKemModel>();
        }

        //update 18/05/2020
        public bool TaoTaiKhoan { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public string PasswordNew { get; set; }
        public string PasswordNewConfirm { get; set; }

        public bool PasswordChange { get; set; }

        public bool IsUpdateView { get; set; }
        //

        public string HoTen { get; set; }
        public long UserId { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string SoCMT { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Avatar { get; set; }
       // public string QuyenHan { get; set; }
        public string GhiChu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string Email { get; set; }
      //  public List<long> Roles { get; set; }
        //public List<long> QuyenHans { get; set; }
        public string TextTenQuyenHan { get; set; }
        public long? HocHamHocViId { get; set; }
        public string TextTenHocHamHocVi { get; set; }

        public long? PhamViHanhNgheId { get; set; }
        public string TextTenPhamViHanhNghe { get; set; }

        public long? ChucDanhId { get; set; }
        public string TextTenChucDanh { get; set; }

        public long? VanBangChuyenMonId { get; set; }
        public string TextTenVanBangChuyenMon { get; set; }
       
        public string MaChungChiHanhNghe { get; set; }
        public DateTime? NgayCapChungChiHanhNghe { get; set; }
        public string NoiCapChungChiHanhNghe { get; set; }
        public DateTime? NgayKyHopDong { get; set; }
        public DateTime? NgayHetHopDong { get; set; }

        public List<long> KhoaPhongIds { get; set; }
        public List<KhoaPhongNhanVienViewModel> KhoaPhongNhanViens { get; set; }

        public List<long> KhoNhanVienQuanLyIds { get; set; }
        public List<KhoNhanVienQuanLyModel> KhoNhanVienQuanLys { get; set; }

        public List<long> PhongBenhVienIds { get; set; }
        public List<long> LstRole { get; set; }
        public long PhongChinhId { get; set; }
        public List<long> ChucVuIds { get; set; }
        public List<HoSoNhanVienFileDinhKemModel> HoSoNhanVienFileDinhKems { get; set; }

        //BVHD-3925
        public string MaNhanVien { get; set; }
        public DateTime? NgayDangKyHanhNghe { get; set; }

    }

    //public class QuyenHan : BaseViewModel
    //{
    //    public long KeyId { get; set; }

    //    public string DisplayName { get; set; }
    //    public bool IsChecked { get; set; }
    //}
    public class HoSoNhanVienFileDinhKemModel :BaseViewModel
    {
        public long NhanVienId { get; set; }
        public string Ma { get; set; }
        public string Uid { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
}
