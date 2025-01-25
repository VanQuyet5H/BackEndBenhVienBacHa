using System.Collections.Generic;
using Camino.Core.Domain;
using iTextSharp.text;

namespace Camino.Api.Models.HopDongThauDuocPham
{
    public class HopDongThauDuocPhamChiTietViewModel : BaseViewModel
    {
        public HopDongThauDuocPhamChiTietViewModel()
        {
            MaDuocPhamTemps = new List<string>();
        }
        public long? HopDongThauDuocPhamId { get; set; }

        public long? DuocPhamId { get; set; }

        public string DuocPham { get; set; }

        public decimal? Gia { get; set; }

        //public decimal GiaBaoHiem { get; set; }

        public double? SoLuong { get; set; }

        public double SoLuongDaCap { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public bool SuDungTaiBenhVien { get; set; }
        //public long? DuocPhamBenhVienPhanNhomId { get; set; }
        //public string DuocPhamBenhVienPhanNhomModelText { get; set; }

        //BVHD-3454
        public long? DuocPhamBenhVienId { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public long? DuocPhamBenhVienPhanNhomConId { get; set; }
        public string TenDuocPhamBenhVienPhanNhomCon { get; set; }
        public long? DuocPhamBenhVienPhanNhomChaId { get; set; }
        public string TenDuocPhamBenhVienPhanNhomCha { get; set; }
        public List<string> MaDuocPhamTemps { get; set; }

        //Số lượng cần nhập
        public double SoLuongCanNhap => (double)SoLuong - SoLuongDaCap;
    }
}
