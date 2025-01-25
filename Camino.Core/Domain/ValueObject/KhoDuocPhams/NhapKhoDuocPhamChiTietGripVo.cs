using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhoDuocPhams
{
    public class NhapKhoDuocPhamChiTietGripVo : GridItem
    {
        //public long DuocPhamBenhVienId { get; set; }
        //public long HopDongThauDuocPhamId { get; set; }
        //public string TenDuocPham { get; set; }
        //public string TenHopDongThau { get; set; }
        //public string SoLo { get; set; }
        //public bool DatChatLuong { get; set; }
        //public string DatChatLuongText { get; set; }
        //public DateTime? HanSuDung { get; set; }
        //public string HanSuDungText { get; set; }

        //public double SoLuongNhap { get; set; }
        //public decimal DonGiaNhap { get; set; }
        //public decimal? DonGiaBan { get; set; }
        //public int? VAT { get; set; }
        //public int? ChietKhau { get; set; }


        //public string TextDonGiaNhap { get; set; }
        //public string TextDonGiaBan { get; set; }
        //public string TextVAT { get; set; }
        //public string TextChietKhau { get; set; }
        //public string TextSoLuongNhap { get; set; }
        //public string  MaVach { get; set; }
        //public long? ViTri { get; set; }
        //public string TenViTri { get; set; }

        //public long? KhoDuocPhamId { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public string TenDuocPham { get; set; }
        public string TenHDThau { get; set; }
        public string NhaThauDisplay { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Loai { get { return LaDuocPhamBHYT ? "BHYT" : "Không BHYT"; } }
        public string Nhom { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay
        {
            get { return HanSuDung.ApplyFormatDate(); }
        }
        public string MaVach { get; set; }
        public double SL { get; set; }
        public string SLDisplay { get; set; }
        public decimal GiaNhap { get; set; }
        public string GiaNhapDisplay { get; set; }
        public int VAT { get; set; }
        public string ViTri { get; set; }
        public string MaRef { get; set; }
    }
}
