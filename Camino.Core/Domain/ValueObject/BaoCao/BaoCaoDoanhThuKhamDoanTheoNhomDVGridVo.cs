using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoDoanhThuKhamDoanTheoNhomDVGridVo : GridItem
    {
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public int NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public decimal KhamBenh { get; set; }
        public decimal XetNghiem { get; set; }
        public decimal NoiSoi { get; set; }
        public decimal NoiSoiTMH { get; set; }
        public decimal SieuAm { get; set; }
        public decimal XQuang { get; set; }
        public decimal CTScan { get; set; }
        public decimal MRI { get; set; }
        public decimal DienTimDienNao { get; set; }
        public decimal TDCNDoLoangXuong { get; set; }
        public decimal DVKhac { get; set; }
        public decimal Total => KhamBenh + XetNghiem + NoiSoi + NoiSoiTMH + SieuAm + XQuang + CTScan + MRI + DienTimDienNao + TDCNDoLoangXuong + DVKhac;
        public long? CongTyId { get; set; }
        public string TenCongTy { get; set; }
    }
}