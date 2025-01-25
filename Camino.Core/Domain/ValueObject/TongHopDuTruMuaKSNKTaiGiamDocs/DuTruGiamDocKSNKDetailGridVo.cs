using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaKSNKTaiGiamDocs
{
    public class DuTruGiamDocKSNKDetailGridVo : GridItem
    {
        public DuTruGiamDocKSNKDetailGridVo()
        {
            TongTonList = new List<TonKSNKChuaNhap>();
            HdChuaNhapList = new List<TonKSNKChuaNhap>();
        }

        public string Loai => IsBhyt ? "BHYT" : "Không BHYT";

        public bool IsBhyt { get; set; }

        public long VatTuId { get; set; }

        public string VatTu { get; set; }

        public string QuyCach { get; set; }

        public string Dvt { get; set; }

        public string NhaSx { get; set; }

        public string NuocSx { get; set; }

        public int SoLuongDuTru { get; set; }

        public int SoLuongDuKienTrongKy { get; set; }

        public int SoLuongDuTruTrKhoa { get; set; }

        public int SoLuongDuTruKhDuoc { get; set; }

        public int? SoLuongDuTruDirector { get; set; }

        public double KhoDuTruTon { get; set; }

        public double KhoTongTon { get; set; }

        public double HdChuaNhap { get; set; }

        public List<TonKSNKChuaNhap> TongTonList { get; set; }

        public List<TonKSNKChuaNhap> HdChuaNhapList { get; set; }
    }

    public class TonKSNKChuaNhap
    {
        public string Name { get; set; }

        public double TongTon { get; set; }
    }
}
