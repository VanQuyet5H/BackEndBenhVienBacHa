using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoKhacBangTheoDoiGayMeHoiSucViewModel : BaseViewModel
    {
        public HoSoKhacBangTheoDoiGayMeHoiSucViewModel()
        {
            FilesChuKy = new List<FileChuKyBangTheoDoiGayMeHoiSucViewModel>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTimeSACH();
        public LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHienDisplay { get; set; }
        public long? NoiThucHienId { get; set; }
        public string NoiThucHienDisplay { get; set; }
        public List<FileChuKyBangTheoDoiGayMeHoiSucViewModel> FilesChuKy { get; set; }

        #region ThongTinHoSo
        public string ChanDoan { get; set; }
        public string TienMe { get; set; }
        public string TacDung { get; set; }
        public string LoaiMo { get; set; }
        public string TuTheMo { get; set; }
        public string NguoiGayMe { get; set; }
        public string NguoiMo { get; set; }
        public string PhuongPhapVoCam { get; set; }
        public string NguoiThuCheo { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public int? Nang { get; set; }
        public int? Cao { get; set; }
        public string KetLuan { get; set; }
        public string MaSoThongTinVienPhi { get; set; }
        #endregion
    }

    public class FileChuKyBangTheoDoiGayMeHoiSucViewModel : BaseViewModel
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
