using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuCongKhaiDichVuKyThuat
{
    public class PhieuCongKhaiDichVuKyThuatViewModel : BaseViewModel
    {
        public PhieuCongKhaiDichVuKyThuatViewModel()
        {
            FileChuKy = new List<FileChuKyPhieuCongKhaiDVKTModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiDVKTModel> FileChuKy { get; set; }
    }

    public class FileChuKyPhieuCongKhaiDVKTModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Uid { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichVatTu { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }

    public class PhieuCongKhaiThuocVatTuViewModel : BaseViewModel
    {
        public PhieuCongKhaiThuocVatTuViewModel()
        {
            FileChuKy = new List<FileChuKyPhieuCongKhaiThuocVatTuModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiThuocVatTuModel> FileChuKy { get; set; }
    }

    public class FileChuKyPhieuCongKhaiThuocVatTuModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Uid { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichVatTu { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
}
