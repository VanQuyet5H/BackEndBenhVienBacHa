using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaThuocTaiGiamDocs
{
    public class DuTruGiamDocDetailGridVo : GridItem
    {
        public DuTruGiamDocDetailGridVo()
        {
            TongTonList = new List<TonChuaNhap>();
            HdChuaNhapList = new List<TonChuaNhap>();
        }

        public string Loai => IsBhyt ? "BHYT" : "Không BHYT";

        public bool IsBhyt { get; set; }

        public long DuocPhamId { get; set; }

        public string DuocPham { get; set; }

        public string HoatChat { get; set; }

        public string NongDo { get; set; }

        public string Sdk { get; set; }

        public string Dvt { get; set; }

        public string DuongDung { get; set; }

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

        public List<TonChuaNhap> TongTonList { get; set; }

        public List<TonChuaNhap> HdChuaNhapList { get; set; }
    }

    public class TonChuaNhap
    {
        public string Name { get; set; }

        public double TongTon { get; set; }
    }
}
