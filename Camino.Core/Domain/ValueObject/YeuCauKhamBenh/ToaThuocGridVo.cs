using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class ToaThuocGridVo : Grid.GridItem
    {
       public string Duoc { get; set; }
        public string HoatChat { get; set; }
        public string DonViTinh { get; set; }
          public string Sang { get; set; }
        public string Trua { get; set; }
        public string Toi { get; set; }
        public string Chieu { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public int? SoNgay { get; set; }
        public double SoLuong { get; set; }
        public string DuongDung { get; set; }
        public string GhiChu { get; set; }

        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => DonGia * (decimal)SoLuong;
        public string BaoHiemTra { get; set; }
        public decimal? BHChiTra { get; set; }
        public decimal? BNChiTra { get; set; }
        public string ThuocBHYT { get; set; }
        public string TuongTacThuoc { get; set; }
        public string DiUngThuoc { get; set; }
        public int LoaiThuocId { get; set; }
        public string TenLoaiThuoc { get; set; }
        public string DiUngThuocDisplay { get; set; }
        public string LoiDan { get; set; }
    }
}

