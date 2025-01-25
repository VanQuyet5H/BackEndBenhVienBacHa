using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhBenhNhanViewModel: BaseViewModel
    {
        public KhamBenhBenhNhanViewModel()
        {
            KhamBenhBenhNhanTienSuBenhs = new List<KhamBenhBenhNhanTienSuBenhViewModel>();
            KhamBenhBenhNhanDiUngThuocs = new List<KhamBenhBenhNhanDiUngThuocViewModel>();
            YeuCauTiepNhans = new List<KhamBenhYeuCauTiepNhanViewModel>();
        }
        public string BHYTMaSoThe { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? BHYTNoiDangKyId { get; set; }
        public string NoiDangKyBHYT { get; set; }
        public string BHYTDiaChi { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string SoChungMinhThu { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public long? DanTocId { get; set; }
        public long? QuocTichId { get; set; }
        public long? TinhThanhId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? PhuongXaId { get; set; }
        public string DiaChi { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }
        public string DanTocModelText { get; set; }
        public string QuocTichModelText { get; set; }
        public string TinhThanhModelText { get; set; }
        public string QuanHuyenModelText { get; set; }
        public string PhuongXaModelText { get; set; }
        public string NgheNghiepModelText { get; set; }
        public string NguoiLienHeThanNhanModelText { get; set; }
        public string BHYTCoQuanBHXH { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public string BHYTMaDKBD { get; set; }
        public string MaBN { get; set; }

        public List<KhamBenhBenhNhanDiUngThuocViewModel> KhamBenhBenhNhanDiUngThuocs { get; set; }
        public List<KhamBenhBenhNhanTienSuBenhViewModel> KhamBenhBenhNhanTienSuBenhs { get; set; }
        public List<KhamBenhYeuCauTiepNhanViewModel> YeuCauTiepNhans { get; set; }
    }
}
