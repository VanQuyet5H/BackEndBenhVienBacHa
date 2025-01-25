using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoKhacGiayChuyenTuyenViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTimeSACH();
        public LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHienDisplay { get; set; }
        public long? NoiThucHienId { get; set; }
        public string NoiThucHienDisplay { get; set; }

        #region ThongTinHoSo
        public string SoHoSo { get; set; }
        public string SoChuyenTuyenSo { get; set; }
        public string NguoiNhan { get; set; }
        public string DauHieuLamSan { get; set; }
        public string KetQuaXetNghiemCLS { get; set; }
        public string ChanDoan { get; set; }
        public string PhuongPhapSuDungTrongDieuTri { get; set; }
        public string TinhTrangNguoiBenh { get; set; }
        public string HuongDieuTri { get; set; }
        public int? LyDoChuyenTuyen { get; set; }
        public DateTime? ChuyenTuyenHoi { get; set; }
        public string PhuongTienVanChuyen { get; set; }
        public string HoTenNguoiHoTong { get; set; }
        public string ChucDanhNguoiHoTong { get; set; }
        public string TrinhDoChuyenMonNguoiHoTong { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public long? NguoiCoThamQuyenId { get; set; }
        public string NguoiCoThamQuyenDisplay { get; set; }
        public long? YBacSiKhamDieuTriId { get; set; }
        public string YBacSiKhamDieuTriDisplay { get; set; }
        #endregion
    }
}
