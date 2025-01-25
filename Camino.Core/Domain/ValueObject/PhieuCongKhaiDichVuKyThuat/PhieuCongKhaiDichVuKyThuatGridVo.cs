using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat
{
    public class PhieuCongKhaiDichVuKyThuatGridVo : GridItem
    {
        public int STT { get; set; }
        public string TenDichVuKyThuat { get; set; }
        public string DonVi { get; set; }
        public DateTime NgayThang { get; set; }
        public List<DichVuKyThuatNgayThang> NgayThangSlDichVuKyThuats { get; set; }
        public double SoLuong { get; set; }
        public double TongSo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }
    }
    public class DichVuKyThuatNgayThang : GridItem
    {
        public DateTime NgayThang { get; set; }
        public double SoLuong { get; set; }
    }
    public class ListDichVuKyThuat
    {
        public ListDichVuKyThuat()
        {
            ListDichVuKyThuatCongKhaiDichVuKyThuat = new List<PhieuCongKhaiDichVuKyThuatGridVo>();
        }
        public List<PhieuCongKhaiDichVuKyThuatGridVo> ListDichVuKyThuatCongKhaiDichVuKyThuat { get; set; }
    }
    public class ListDichVuKyThuatPhieu : GridItem
    {
        public List<ListDichVuKyThuat> ListDichVuKyThuatPhieuCongKhai { get; set; }
    }
    public class ListDichVuKyThuatPhieus
    {
        public List<ListDichVuKyThuat> ListDichVuKyThuatPhieuCongKhai { get; set; }
    }
    public class ListIdPhieuDieuTri
    {
        public List<long> Id { get; set; }
    }
    public class ListDatePhieu : GridItem
    {
        public List<DateTime> ItemColumns { get; set; }
    }
    public class ListPhieuStringObject
    {
        public List<string> ListStringObject { get; set; }
        public decimal? TongSo { get; set; }
        public decimal? TongTien { get; set; }
        public decimal? DonGia { get; set; }
    }
    public class ListTemplateObjectGridVo
    {
        public List<string> ListTemplate { get; set; }
        public decimal TongSo { get; set; }
        public decimal TongTien { get; set; }
        public decimal DonGia { get; set; }
    }
    public class ListTotalColumnTable
    {
        public DateTime DateColumn { get; set; }
        public double? SlColumn { get; set; }
    }
    public class ListBottomTotalColumn : GridItem
    {
        public ListBottomTotalColumn()
        {
            ListtTotalColumn = new List<ListTotalColumnTable>();
        }
        public List<ListTotalColumnTable> ListtTotalColumn { get; set; }
    }
    public class ListTable : GridItem
    {
        public ListPhieuStringObject ListTables { get; set; }
    }
}
