using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuCongKhaiVatTu
{
    public class PhieuCongKhaiVatTuGridVo : GridItem
    {
        public int STT { get; set; }
        public string TenVatTu { get; set; }
        public string DonVi { get; set; }
        public DateTime NgayThang { get; set; }
        public List<VatTuNgayThang> NgayThangSlVatTus { get; set; }
        public double SoLuong { get; set; }
        public double TongSo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }

        //BVHD-3876
        public long? KhoaId { get; set; }
        public string TenKhoa { get; set; }
    }
    public class VatTuNgayThang : GridItem
    {
        public DateTime NgayThang { get; set; }
        public double SoLuong { get; set; }
    }
    public class ListVatTu
    {
        public ListVatTu()
        {
            ListVatTuCongKhaiVatTu = new List<PhieuCongKhaiVatTuGridVo>();
        }
        public List<PhieuCongKhaiVatTuGridVo> ListVatTuCongKhaiVatTu { get; set; }
    }
    public class ListVatTuPhieu : GridItem
    {
        public List<ListVatTu> ListVatTuPhieuCongKhai { get; set; }
    }
    public class ListVatTuPhieus
    {
        public List<ListVatTu> ListVatTuPhieuCongKhai { get; set; }
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
