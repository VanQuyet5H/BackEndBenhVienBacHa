using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class ThongKeThuocTheoBacSiQueryInfo : QueryInfo
    {
        public long BacSiId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }
    public class ThongKeThuocTheoBacSiDataVo
    {
        public bool LaThuocBHYT { get; set; }
        public bool NoiTru { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? SoTienMienGiam { get; set; }

        public decimal DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public int? TiLeTheoThapGia { get; set; }
        public List<Enums.PhuongPhapTinhGiaTriTonKho> PhuongPhapTinhGiaTriTonKhos { get; set; }        
    }
    public class DanhSachThongKeThuocTheoBacSi : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public bool LaThuocBHYT { get; set; }
        public bool NoiTru { get; set; }
        public string Loai => LaThuocBHYT ? "BHYT" : "Viện phí";     

        public string TenThuocHamLuong { get; set; }

        public string DonViTinh { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string LoaiNoiTru => NoiTru ? "Nội trú" : "Ngoại trú";
    }
}
