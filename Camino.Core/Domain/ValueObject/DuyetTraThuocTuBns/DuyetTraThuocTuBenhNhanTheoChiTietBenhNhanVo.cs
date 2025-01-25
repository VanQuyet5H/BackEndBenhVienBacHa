using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns
{
    public class DuyetTraThuocTuBenhNhanTheoChiTietBenhNhanVo : GridItem
    {
        public DateTime? NgayDieuTri { get; set; }

        public string NgayDieuTriDisplay => NgayDieuTri?.ApplyFormatDate();

        public DateTime? NgayTra { get; set; }

        public string NgayTraDisplay => NgayTra?.ApplyFormatDate();

        public string BenhNhan => GetBenhNhanTemplate();

        public string NguoiTra { get; set; }

        public double? SoLuongChiDinh { get; set; }

        public string SlChiDinh => Math.Abs(SoLuongChiDinh.GetValueOrDefault()) > 0 ? SoLuongChiDinh.GetValueOrDefault().ApplyNumber() : 0.ToString();

        public double? SoLuongDaTra { get; set; }

        public string SlDaTra => Math.Abs(SoLuongDaTra.GetValueOrDefault()) > 0 ? SoLuongDaTra.GetValueOrDefault().ApplyNumber() : 0.ToString();

        public double? SoLuongTraLanNay { get; set; }

        public string SlTraLanNay => Math.Abs(SoLuongTraLanNay.GetValueOrDefault()) > 0 ? SoLuongTraLanNay.GetValueOrDefault().ApplyNumber() : 0.ToString();

        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, Vat);

        public decimal DonGiaNhap { get; set; }

        public int TiLeTheoThapGia { get; set; }

        public int Vat { get; set; }
        public bool? KhongTinhPhi { get; set; }

        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuongTraLanNay.GetValueOrDefault() : 0;

        public string MaTn { get; set; }

        public string MaBn { get; set; }

        public string HoTen { get; set; }

        private string GetBenhNhanTemplate()
        {
            return "MÃ TN: <b>" + MaTn + "</b> - "
                   + "MÃ BN: <b>" + MaBn + "</b> - "
                   + " HỌ TÊN: <b>" + HoTen + "</b> - "
                   + "SL CHỈ ĐỊNH: <b>" + SlChiDinh + "</b> - "
                   + "SL ĐÃ TRẢ: <b>" + SlDaTra + "</b>";
        }

        public bool? DuocDuyet { get; set; }
    }
}
