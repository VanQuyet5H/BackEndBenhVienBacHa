using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuTheoDoiTruyenMau
{
    public class PhieuTheoDoiTruyenMauViewModel : BaseViewModel
    {
        public PhieuTheoDoiTruyenMauViewModel()
        {
            FileChuKy = new List<FileChuKyPhieuTheoDoiTruyenMauModel>();
            //ChiSoSinhTons= new List<ChiSoSinhTonModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyPhieuTheoDoiTruyenMauModel> FileChuKy { get; set; }
        public long? MaDonViMauTruyenId { get; set; }
        public string DDTruyenMau { get; set; }
        public long? DDTruyenMauId { get; set; }
        public DateTime? BatDauTruyenHoi { get; set; }
        public DateTime? NgungTruyenHoi { get; set; }
    }
    public class FileChuKyPhieuTheoDoiTruyenMauModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Uid { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class ChiSoSinhTonModel
    {
        public long HuyetAp1 { get; set; }
        public string HuyetAp2 { get; set; }
    }
}
