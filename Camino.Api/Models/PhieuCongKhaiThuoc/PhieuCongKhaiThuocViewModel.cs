using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuCongKhaiThuoc
{
    public class PhieuCongKhaiThuocViewModel : BaseViewModel
    {
        public PhieuCongKhaiThuocViewModel()
        {
            FileChuKy = new List<FileChuKyPhieuCongKhaiThuocModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiThuocModel> FileChuKy { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
    }
    public class FileChuKyPhieuCongKhaiThuocModel
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
    public class ValidatorTuNgayDenNgay
    {
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
    }
}
