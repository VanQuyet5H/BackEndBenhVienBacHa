using Camino.Api.Models.VatTu;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.VatTuBenhViens
{
    public class VatTuBenhVienViewModel : BaseViewModel
    {
        //vat tu benh vien
        public bool? HieuLuc { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public string TenLoaiSuDung { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }
        public string MaVatTuBenhVien { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        // vat tu
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? NhomVatTuId { get; set; }
        public string TenNhomVatTu { get; set; }
        public string DonViTinh { get; set; }
        public int? TyLeBaoHiemThanhToan { get; set; }
        public string QuyCach { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string MoTa { get; set; }
        public bool? SuDungVatTuBenhVien { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
