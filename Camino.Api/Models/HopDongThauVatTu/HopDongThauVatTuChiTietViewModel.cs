using System.Collections.Generic;
using Camino.Core.Domain;
using iTextSharp.text;

namespace Camino.Api.Models.HopDongThauVatTu
{
    public class HopDongThauVatTuChiTietViewModel : BaseViewModel
    {
        public HopDongThauVatTuChiTietViewModel()
        {
            MaVatTuTemps = new List<string>();
        }
        public long? VatTuId { get; set; }

        public string VatTu { get; set; }

        public decimal? Gia { get; set; }

        public double? SoLuong { get; set; }
        public double? SoLuongDaCap { get; set; }

        public string MaVatTuBenhVien { get; set; }
        public Enums.LoaiSuDung? LoaiSuDungId { get; set; }
        public string LoaiSuDungText { get; set; }
        public bool SuDungTaiBenhVien { get; set; }

        //BVHD-3472
        public long? VatTuBenhVienId { get; set; }
        public List<string> MaVatTuTemps { get; set; }

        //Số lượng cần nhập
        public double SoLuongCanNhap => (SoLuong ?? 0) - (SoLuongDaCap ?? 0);
    }
}
