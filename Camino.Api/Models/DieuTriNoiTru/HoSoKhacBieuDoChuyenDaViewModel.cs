using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoKhacBieuDoChuyenDaViewModel : BaseViewModel
    {
        public HoSoKhacBieuDoChuyenDaViewModel()
        {
            FilesChuKy = new List<FileChuKyBieuDoChuyenDaViewModel>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTimeSACH();
        public LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHienDisplay { get; set; }
        public long? NoiThucHienId { get; set; }
        public string NoiThucHienDisplay { get; set; }
        public List<FileChuKyBieuDoChuyenDaViewModel> FilesChuKy { get; set; }

        #region ThongTinHoSo
        public DateTime? NgayGhiBieuDo { get; set; }
        public long? NguoiGhiBieuDoId { get; set; }
        public string NguoiGhiBieuDoDisplay { get; set; }
        public int? TienThaiPara1 { get; set; }
        public int? TienThaiPara2 { get; set; }
        public int? TienThaiPara3 { get; set; }
        public int? TienThaiPara4 { get; set; }
        #endregion
    }

    public class FileChuKyBieuDoChuyenDaViewModel : BaseViewModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
}