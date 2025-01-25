using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class ThongTinDoiTuongTiepNhanViewModel : BaseViewModel
    {
        public ThongTinDoiTuongTiepNhanViewModel()
        {
            YeuCauTiepNhanTheBHYTs = new List<NoiTruYeuCauTiepNhanTheBHYTViewModel>();
            YeuCauTiepNhanCongTyBaoHiemTuNhans = new List<NoiTruYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel>();
        }

        public long BenhNhanId { get; set; }
        public bool? CoBHYT { get; set; }
        public bool? TuNhap { get; set; }
        public bool? CoBHTN { get; set; }

        public string HoTen { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public long? QuocTichId { get; set; }
        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public long? DanTocId { get; set; }

        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }
        public string NguoiLienHeDiaChi { get; set; }
        public long? NguoiLienHePhuongXaId { get; set; }
        public long? NguoiLienHeQuanHuyenId { get; set; }
        public long? NguoiLienHeTinhThanhId { get; set; }

        public string BHYTMaSoThe { get; set; }
        public string NoiDangKyBHYT { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTDiaChi { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public string BHYTMaDKBD { get; set; }
        public int? BHYTMucHuong { get; set; }

        public string HoTenBo { get; set; }
        public string TrinhDoVanHoaCuaBo { get; set; }
        public long? NgheNghiepCuaBoId { get; set; }
        public string HoTenMe { get; set; }
        public string TrinhDoVanHoaCuaMe { get; set; }
        public long? NgheNghiepCuaMeId { get; set; }
        public long? YeuCauTiepNhanMeId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }

        public bool? IsCheckedBHYT { get; set; }

        public List<NoiTruYeuCauTiepNhanTheBHYTViewModel> YeuCauTiepNhanTheBHYTs { get; set; }
        public List<NoiTruYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel> YeuCauTiepNhanCongTyBaoHiemTuNhans { get; set; }
        public string BieuHienLamSang { get; set; }
        public string DichTeSarsCoV2 { get; set; }
    }

    public class KiemTraValidationListTheBHYTViewModel : BaseViewModel
    {
        public KiemTraValidationListTheBHYTViewModel()
        {
            YeuCauTiepNhanTheBHYTs = new List<NoiTruYeuCauTiepNhanTheBHYTViewModel>();
        }
        
        public List<NoiTruYeuCauTiepNhanTheBHYTViewModel> YeuCauTiepNhanTheBHYTs { get; set; }
    }
}
