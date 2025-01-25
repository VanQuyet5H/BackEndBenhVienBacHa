using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using Camino.Api.Models.KhoaPhongNhanVien;
namespace Camino.Api.Models.NhanVien
{
    public class ProfileNhanVien : BaseViewModel
    {
        public ProfileNhanVien()
        {
            KhoaPhongNhanViens = new List<KhoaPhongNhanVienViewModel>();
            HoSoNhanVienFileDinhKems = new List<HoSoNhanVienFileDinhKemModel>();
        }
        public string HoTen { get; set; }
        public long UserId { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string SoCMT { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Avatar { get; set; }
        public string GhiChu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string Email { get; set; }
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
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }        
        public DateTime? NgayKyHopDong { get; set; }
        public DateTime? NgayHetHopDong { get; set; }
        public List<long> KhoaPhongIds { get; set; }
        public List<KhoaPhongNhanVienViewModel> KhoaPhongNhanViens { get; set; }

        public List<long> LstRole { get; set; }
        public List<long> ChucVuIds { get; set; }
        public List<HoSoNhanVienFileDinhKemModel> HoSoNhanVienFileDinhKems { get; set; }
    }
}
