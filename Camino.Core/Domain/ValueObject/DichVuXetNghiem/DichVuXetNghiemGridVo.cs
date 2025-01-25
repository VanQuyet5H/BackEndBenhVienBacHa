using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DichVuXetNghiem
{
    public class DichVuXetNghiemGridVo : GridItem
    {
        public DichVuXetNghiemGridVo()
        {
            ChiSoXNChild = new List<DichVuXetNghiemGridVo>();
        }
        public string IdCap { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenDichVuKyThuat { get; set; }
        public string TenCha { get; set; }
        public string ChiSoCha { get; set; }
        public EnumLoaiChiSoXetNghiem? Loai { get; set; }
        public EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }
        public string TenLoaiMauXetNghiem => LoaiMauXetNghiem?.GetDescription();
        public long? DichVuXetNghiemChaId { get; set; }
        public long? DichVuXetNghiemId { get; set; }
        public int? CapDichVu { get; set; }
        public string MoTa { get; set; }
        public string SearchString { get; set; }
        public bool HasChildren { get; set; }
        public virtual List<DichVuXetNghiemGridVo> ChiSoXNChild { get; set; }
        public bool? CoChiSoXetNghiem { get; set; }


        ///Export
        ///
        public string TenMauMay { get; set; }
        public string MaKetNoi { get; set; }
        public string DonVi { get; set; }
        public int? SoThuTu { get; set; }
        public string NamMin { get; set; }
        public string NamMax { get; set; }
        public string NuMin { get; set; }
        public string NuMax { get; set; }
        public string TreEmMin { get; set; }
        public string TreEmMax { get; set; }
        public string NguyHiemMax { get; set; }
        public string NguyHiemMin { get; set; }
        public string TreEm6Min { get; set; }
        public string TreEm6Max { get; set; }
        public string TreEm612Min { get; set; }
        public string TreEm612Max { get; set; }
        public string TreEm1218Min { get; set; }
        public string TreEm1218Max { get; set; }
    }
    public class DichVuKyThuatBenhVienGridVo : GridItem
    {
        public EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }
    }

    public class MauMayXetNghiemGridVo
    {
        public long Id { get; set; }
        public long MauMayXetNghiemId { get; set; }
        public string TenMauMayXN { get; set; }
        public string TenKetNoi { get; set; }
        public string MaChiSo { get; set; }
        public bool? NotSendOrder { get; set; }

    }

    public class MauMayXetNghiemLookup
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public int? Rank { get; set; }
    }

    public class ExportDichVuXetNghiemGridVo : GridItem
    {
        public ExportDichVuXetNghiemGridVo()
        {
            DichVuXetNghiemCons = new List<ExportDichVuXetNghiemGridVo>();
        }
        public long NhomDichVuBenhVienId { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public string Ten { get; set; }
        public string TenMauMay { get; set; }
        public long? DichVuXetNghiemChaId { get; set; }
        public string MaDichVuXetNghiem { get; set; }
        public int? CapDichVu { get; set; }
        public string CapDichVuDisplay { get; set; }
        public string LoaiMauXetNghiem { get; set; }
        public string MaKetNoi { get; set; }
        public string DonVi { get; set; }
        public int? SoThuTu { get; set; }
        public string NamMin { get; set; }
        public string NamMax { get; set; }
        public string NuMin { get; set; }
        public string NuMax { get; set; }
        public string TreEmMin { get; set; }
        public string TreEmMax { get; set; }
        public string NguyHiemMax { get; set; }
        public string NguyHiemMin { get; set; }
        public string KieuDuLieu { get; set; }
        public string TreEm6Min { get; set; }
        public string TreEm6Max { get; set; }
        public string TreEm612Min { get; set; }
        public string TreEm612Max { get; set; }
        public string TreEm1218Min { get; set; }
        public string TreEm1218Max { get; set; }
        public virtual List<ExportDichVuXetNghiemGridVo> DichVuXetNghiemCons { get; set; }
    }
}
