using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.TonKhos
{
    public class VatTuSapHetHanGridVo : GridItem
    {
        public string TenKho { get; set; }
        public long? VitriId { get; set; }
        public long? KhoId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public string ViTri { get; set; }
        public DateTime NgayHetHan { get; set; }
        public int SoNgayTruocKhiHetHan { get; set; }
        public string NgayHetHanHienThi { get; set; }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon == 0 ? SoLuongTon.ToString() : SoLuongTon.ApplyNumber();
        public string MaVatTu { get; set; }
        public string SoLo { get; set; }
        public decimal DonGiaNhap { get; set; }
        public long NhapKhoVatTuId { get; set; }
        public double ThanhTien => SoLuongTon != 0 && DonGiaNhap != 0 ? (SoLuongTon * Convert.ToDouble(DonGiaNhap)).MathRoundNumber(2) : 0;
    }

    public class VatTuSapHetHanSearchGridVoItem : GridItem
    {
        public VatTuSapHetHanSearchGridVoItem()
        {
            Sort = new List<Sort>();
        }
        public long KhoId { get; set; }
        public string SearchString { get; set; }
        public List<Sort> Sort { get; set; }
        public string SortStringFormat { get; set; }
        public string SortString
        {
            get
            {
                if (!string.IsNullOrEmpty(SortStringFormat))
                {
                    return SortStringFormat;
                }
                // order the results
                if (Sort != null && Sort.Count > 0)
                {
                    var sorts = new List<string>();
                    Sort.ForEach(x => sorts.Add(string.Format("{0} {1}", x.Field, x.Dir)));
                    return string.Join(",", sorts.ToArray());
                }
                return string.Empty;
            }
        }
        //public string VatTu { get; set; }
    }
}
