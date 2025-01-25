using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.NhapKhoMarketting
{
    public class ThongTinNhapKhoQuaTangGridVo : GridItem
    {
        public ThongTinNhapKhoQuaTangGridVo()
        {
            DanhSachQuaTangDuocThemList = new List<DanhSachQuaTangDuocThemGridVo>();
        }

        public string SoChungTu { get; set; }
        public Enums.LoaiNguoiGiaoNhan? LoaiNguoiGiao { get; set; }
        public string TenNguoiGiao { get; set; }
        public DateTime? NgayNhap { get; set; }
        public long? NguoiGiaoId { get; set; }
        public long? NguoiNhapId { get; set; }
        public string TenNguoiNhap { get; set; }
        public string NhaCungCap { get; set; }
        public string QuaTang { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuong { get; set; }
        public double? GiaNhap { get; set; }
        public double ThanhTien { get; set; }
        public List<long> ListNhapKhoQuaTangChiTietId { get; set; }
        public List<DanhSachQuaTangDuocThemGridVo> DanhSachQuaTangDuocThemList { get; set; }
    }
    public class DanhSachQuaTangDuocThemGridVo : GridItem
    {
        public long NhapKhoQuaTangId { get; set; }
        public long QuaTangId { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int SoLuongDaXuat { get; set; }
        public string NhaCungCap { get; set; }
        public string QuaTang { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
