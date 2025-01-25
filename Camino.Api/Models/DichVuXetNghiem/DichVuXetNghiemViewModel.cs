
using System.Collections.Generic;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DichVuXetNghiem
{
    public class DichVuXetNghiemViewModel : BaseViewModel
    {
        public DichVuXetNghiemViewModel()
        {
            KetNoiChiSoXetNghiems = new List<KetNoiChiSoXetNghiemViewModel>();
        }
        public long? NhomDichVuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenCha { get; set; }
        public string TenKetNoi { get; set; }
        public string TenChiSo { get; set; }

        public string TenDichVuKyThuat { get; set; }
        public int? CapDichVu { get; set; }
        public EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }
        public string TenLoaiMauXetNghiem { get; set; }
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
        public bool? CoChiSoXetNghiem { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuXetNghiemChaId { get; set; }
        public long? DichVuXetNghiemId { get; set; }
        public bool HieuLuc { get; set; }
        public bool IsCreateChild { get; set; }
        public string ChiSoCha { get; set; }
        public bool IsDelete { get; set; }

        public string MaKetNoi { get; set; }
        public double? TiLe { get; set; }

        //BVHD-3901
        public long? HdppId { get; set; }
        public string HdppName { get; set; }
        public bool? LaChuanISO { get; set; }

        public List<KetNoiChiSoXetNghiemViewModel> KetNoiChiSoXetNghiems { get; set; }
    }
    public class KetNoiChiSoXetNghiemViewModel
    {
        public long Id { get; set; }
        public string TenKetNoi { get; set; }
        public string MaChiSo { get; set; }
        public long? MauMayXetNghiemId { get; set; }
        public string TenMauMayXetNghiem { get; set; }
        public bool? NotSendOrder { get; set; }

    }


    public class ChuaCoDichVuXetNghiemViewModel
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenCha { get; set; }
        public string TenDichVuKyThuat { get; set; }
        public int? CapDichVu { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }
        public string TenLoaiMauXetNghiem => LoaiMauXetNghiem?.GetDescription();
    }

    public class DichVuXetNghiemKetNoiChiSoViewModel : BaseViewModel
    {
        public string MaKetNoi { get; set; }
        public string TenKetNoi { get; set; }
        public string MaChiSo { get; set; }
        public long? MauMayXetNghiemId { get; set; }
        public bool HieuLuc { get; set; }
        public double TiLe { get; set; }
        public bool? NotSendOrder { get; set; }
    }
}
