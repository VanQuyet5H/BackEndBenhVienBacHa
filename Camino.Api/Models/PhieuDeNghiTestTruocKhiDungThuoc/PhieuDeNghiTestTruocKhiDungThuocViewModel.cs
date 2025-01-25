using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuDeNghiTestTruocKhiDungThuoc
{
    public class PhieuDeNghiTestTruocKhiDungThuocViewModel : BaseViewModel
    {
        public PhieuDeNghiTestTruocKhiDungThuocViewModel()
        {
            FileChuKy = new List<FileChuKyPhieuDeNghiTestTruocKhiDungThuocModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyPhieuDeNghiTestTruocKhiDungThuocModel> FileChuKy { get; set; }
    }
    public class FileChuKyPhieuDeNghiTestTruocKhiDungThuocModel
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
}
