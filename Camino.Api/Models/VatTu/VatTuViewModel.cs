
using Camino.Core.Domain;

namespace Camino.Api.Models.VatTu
{
    public class VatTuViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? NhomVatTuId { get; set; }
        public string NhomVatTuModelText { get; set; }
        public string DonViTinh { get; set; }
        public int? TyLeBaoHiemThanhToan { get; set; }
        public string QuyCach { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string MoTa { get; set; }
        public bool IsDisabled { get; set; }
        public bool? SuDungVatTuBenhVien { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public string LoaiSuDungText { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }
        public string MaVatTuBenhVien { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
        public VatTuBenhViewModel VatTuBenhViewModel { get; set; }

        //BVHD-3472
        public bool? ChuaTaoVatTuBenhVien { get; set; }
    }
    public class VatTuBenhViewModel : BaseViewModel
    {
        public string MaVatTuBenhVien { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
    }

    public class DanhSachNhapVatTuExcelError
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int TotalThanhCong { get; set; }
        public string Error { get; set; }
    }
}
