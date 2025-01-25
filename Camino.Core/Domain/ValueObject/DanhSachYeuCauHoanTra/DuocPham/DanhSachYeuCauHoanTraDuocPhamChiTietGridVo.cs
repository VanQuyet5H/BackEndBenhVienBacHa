using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.DuocPham
{
    public class DanhSachYeuCauHoanTraDuocPhamChiTietGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string DuocPham { get; set; }

        public string HopDong { get; set; }
        public string Ma { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBhyt { get; set; }

        public string Loai => GetLoaiBhyt(LaDuocPhamBhyt);

        private string GetLoaiBhyt(bool laBhyt)
        {
            return laBhyt ? "BHYT" : "Không BHYT";
        }

        public string SoLo { get; set; }

        public DateTime HanSuDung { get; set; }

        public DateTime NgayNhapVaoBenhVien { get; set; }

        public string HsdText => HanSuDung.ApplyFormatDate();

        public string NgayNhapBvText => NgayNhapVaoBenhVien.ApplyFormatDate();

        public decimal DonGiaNhap { get; set; }

        public int TiLeThapGia { get; set; }

        public int Vat { get; set; }

        public string MaVach { get; set; }

        public string Nhom { get; set; }
        public string TenNhom { get; set; }

        public double SoLuongTra { get; set; }
    }
    public class DanhSachDaDuyetChiTietVo
    {
        public long YeuCauTraDuocPhamId { get; set; }
        public string SearchString { get; set; }
    }
}
