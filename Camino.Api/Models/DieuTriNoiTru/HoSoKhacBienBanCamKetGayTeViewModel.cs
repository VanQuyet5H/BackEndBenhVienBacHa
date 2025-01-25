using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel : BaseViewModel
    {
        public HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel()
        {
            FilesChuKy = new List<FileChuKyBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>();
            QuanHeThanNhans = new List<ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel>();
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
        public List<FileChuKyBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel> FilesChuKy { get; set; }

        #region ThongTinHoSo
        public string BacSiGiaiThich { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public long? BacSiGMHSId { get; set; }
        public List<ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel> QuanHeThanNhans { get; set; }
        #endregion
    }

    public class ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel
    {
        public string HoTen { get; set; }
        public string CMND { get; set; }
        public long? QuanHeId { get; set; }
        public string DiaChi { get; set; }
    }

    public class FileChuKyBienBanCamKetGayTeGiamDauTrongDeSauMoViewModel : BaseViewModel
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
