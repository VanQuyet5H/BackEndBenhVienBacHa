using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.XetNghiem
{
    public class LayMauXetNghiemViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public int? BarcodeNumber { get; set; }
        public string BarcodeId { get; set; }
    }

    public class PhieuGuiMauXetNghiemViewModel : BaseViewModel
    {
        public PhieuGuiMauXetNghiemViewModel()
        {
            NhomMauGuis = new List<LayMauXetNghiemViewModel>();
        }
        public long? NhanVienGuiMauId { get; set; }
        public DateTime? ThoiDiemGuiMau { get; set; }
        public long? PhongNhanMauId { get; set; }
        public string GhiChu { get; set; }
        public List<LayMauXetNghiemViewModel> NhomMauGuis { get; set; }
    }

    public class CapBarcodeTheoDichVuViewModel
    {
        public CapBarcodeTheoDichVuViewModel()
        {
            YeuCauDichVuKyThuatIds = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public int? BarcodeNumber { get; set; }
        public string BarcodeId { get; set; }
        public int? SoLuong { get; set; }
        public int? SoLuongThem { get; set; }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
        // BVHD-3836
        public long? NhanVienLayMauId { get; set; }
        public DateTime? ThoiGianLayMau { get; set; }
    }

    public class XacNhanNhanMauChoDichVuViewModel
    {
        public XacNhanNhanMauChoDichVuViewModel()
        {
            YeuCauDichVuKyThuatIds = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
    }

    public class CapNhatGridItemChoDichVuDaCapCodeViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public DateTime? NgayNhanMau { get; set; }
    }
}
