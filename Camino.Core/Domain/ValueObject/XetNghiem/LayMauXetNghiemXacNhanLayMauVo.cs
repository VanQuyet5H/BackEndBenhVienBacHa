using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class LayMauXetNghiemXacNhanLayMauVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public int BarcodeNumber { get; set; }
        public string BarcodeId { get; set; }
    }

    public class CapBarcodeTheoDichVuVo
    {
        public CapBarcodeTheoDichVuVo()
        {
            YeuCauDichVuKyThuatIds = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public int BarcodeNumber { get; set; }
        public string BarcodeId { get; set; }
        public int? SoLuong { get; set; }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
        // BVHD-3836
        public long? NhanVienLayMauId { get; set; }
        public DateTime? ThoiGianLayMau { get; set; }
    }

    public class XacNhanNhanMauChoDichVuVo
    {
        public XacNhanNhanMauChoDichVuVo()
        {
            YeuCauDichVuKyThuatIds = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
    }

    public class CapNhatGridItemChoDichVuDaCapCodeVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public DateTime NgayNhanMau { get; set; }
    }
}
