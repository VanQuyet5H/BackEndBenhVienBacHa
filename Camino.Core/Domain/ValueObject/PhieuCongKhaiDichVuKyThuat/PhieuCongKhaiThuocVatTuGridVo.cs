using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuCongKhaiThuocVatTu
{
    public class PhieuCongKhaiThuocVatTuGridVo : GridItem
    {
        public int STT { get; set; }
        public string TenThuocVatTu { get; set; }
        public string DonVi { get; set; }
        public DateTime NgayThang { get; set; }
        public List<ThuocVatTuNgayThang> NgayThangSlThuocVatTus { get; set; }
        public double SoLuong { get; set; }
        public double TongSo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public Enums.LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public bool LaDuocPham { get; set; }
    }
    public class ThuocVatTuNgayThang : GridItem
    {
        public DateTime NgayThang { get; set; }
        public double SoLuong { get; set; }
    }
    public class ListThuocVatTu
    {
        public ListThuocVatTu()
        {
            ListThuocVatTuCongKhaiThuocVatTu = new List<PhieuCongKhaiThuocVatTuGridVo>();
        }
        public List<PhieuCongKhaiThuocVatTuGridVo> ListThuocVatTuCongKhaiThuocVatTu { get; set; }
    }
    public class ListThuocVatTuPhieu : GridItem
    {
        public List<ListThuocVatTu> ListThuocVatTuPhieuCongKhai { get; set; }
    }
    public class ListThuocVatTuPhieus
    {
        public List<ListThuocVatTu> ListThuocVatTuPhieuCongKhai { get; set; }
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
