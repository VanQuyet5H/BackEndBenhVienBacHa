using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuTheoDoiTruyenDich
{
    public class PhieuTheoDoiTruyenDichViewModel : BaseViewModel
    {
        public PhieuTheoDoiTruyenDichViewModel()
        {
            FileChuKy = new List<FileChuKyPhieuTheoDoiTruyenDichModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyPhieuTheoDoiTruyenDichModel> FileChuKy { get; set; }
    }
    public class FileChuKyPhieuTheoDoiTruyenDichModel
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
    public class ValidatetorTruyenDich { 
        public ValidatetorTruyenDich()
        {
            listTruyenDich = new List<ValidateSoLuongTruyenDich>();
        }
        public List<ValidateSoLuongTruyenDich> listTruyenDich { get; set; }
    }
    public class ValidateSoLuongTruyenDich
    {
        public string NgayThu { get; set; }

        public double SoLuong { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long DuocPhamId { get; set; }
        public double BatDau { get; set; }
    }
}
