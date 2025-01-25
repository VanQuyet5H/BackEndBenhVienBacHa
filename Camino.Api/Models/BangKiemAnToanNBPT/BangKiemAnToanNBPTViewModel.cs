using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BangKiemAnToanNBPT
{
    public class BangKiemAnToanNBPTViewModel : BaseViewModel
    {
        public BangKiemAnToanNBPTViewModel()
        {
            FileChuKy = new List<FileChuKyBangKiemAnToanModel>();
            ListChiSoSinhTon = new List<ChiSoSinhTonBangKiemAnToanViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyBangKiemAnToanModel> FileChuKy { get; set; }
        public List<ChiSoSinhTonBangKiemAnToanViewModel> ListChiSoSinhTon { get; set; }
        public DateTime? NgayGioDuaBNDiPT { get; set; }
        public DateTime? NgayGioDuDinhGayMe { get; set; }
        public DateTime? ThoiDiemKham { get; set; }
    }
    public class FileChuKyBangKiemAnToanModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long size { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
     
    }
    public class FileChuKyBangKiemAnToanViewModel : BaseViewModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class ChiSoSinhTonBangKiemAnToanViewModel : BaseViewModel
    {
        public int? NhipTim { get; set; }
        public int? NhipTho { get; set; }
        public string HuyetAp { get; set; }
        public double? ThanNhiet { get; set; }
        public double? ChieuCao { get; set; }
        public double? CanNang { get; set; }
        public double? BMI { get; set; }
        public string NhanVienThucHien { get; set; }
        //public string NgayThucHien { get; set; }
        public double? Glassgow { get; set; }
        public double? SpO2 { get; set; }

        public int? HuyetApTamThu { get; set; }
        public int? HuyetApTamTruong { get; set; }

    }
}
