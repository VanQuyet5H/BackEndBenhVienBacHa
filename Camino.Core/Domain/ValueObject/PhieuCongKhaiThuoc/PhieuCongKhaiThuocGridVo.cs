using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuCongKhaiThuoc
{
    public class PhieuCongKhaiThuocGridVo : GridItem
    {
        public int STT { get; set; }
        public string TenThuoc { get; set; }
        public string DonVi { get; set; }
        public DateTime NgayThang { get; set; }
        public List<ThuocNgayThang> NgayThangSlThuocs { get; set; }
        public double SoLuong { get; set; }
        public double TongSo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public Enums.LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }

        //BVHD-3876
        public long? KhoaId { get; set; }
        public string TenKhoa { get; set; }
    }
    public class ThuocNgayThang : GridItem
    {
        public DateTime NgayThang { get; set; }
        public double SoLuong { get; set; }
    }
    public class ListThuoc
    {
        public ListThuoc()
        {
            ListThuocCongKhaiThuoc = new List<PhieuCongKhaiThuocGridVo>();
        }
        public List<PhieuCongKhaiThuocGridVo> ListThuocCongKhaiThuoc { get; set; }
    }
    public class ListThuocPhieu : GridItem
    {
        public List<ListThuoc> ListThuocPhieuCongKhai { get; set; }
    }
    public class ListThuocPhieus
    {
        public List<ListThuoc> ListThuocPhieuCongKhai { get; set; }
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
    public class ListBottomTotalColumn :GridItem
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
