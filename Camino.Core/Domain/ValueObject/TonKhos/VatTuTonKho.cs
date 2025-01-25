using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.TonKhos
{
    public class VatTuTonKhoCanhBaoGridVo : GridItem
    {
        public string TenKho { get; set; }
        //public long? VitriId { get; set; }
        public long? KhoId { get; set; }
        //public long VatTuBenhVienId { get; set; }
        //public string TenVatTu { get; set; }
        //public string DonViTinh { get; set; }
        //public string ViTri { get; set; }
        //public DateTime NgayHetHan { get; set; }
        //public string NgayHetHanHienThi { get; set; }
        //public double SoLuongTon { get; set; }

        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon == 0 ? SoLuongTon.ToString() : SoLuongTon.ApplyNumber();
        public int? TonToiThieu { get; set; }
        public int? TonToiDa { get; set; }
        public string CanhBao
        {
            get
            {
                if (SoLuongTon <= (double)0)
                {
                    return "Hết tồn kho";
                }
                else if (TonToiDa != null && SoLuongTon > TonToiDa)
                {
                    return "Tồn kho quá nhiều";
                }

                else if (TonToiThieu != null && SoLuongTon < TonToiThieu)
                {
                    return "Sắp hết tồn kho";
                }
                return string.Empty;
            }
        }
        public string MauCanhBao
        {
            get
            {
                if (CanhBao.Contains("Hết tồn kho"))
                {
                    return "red";
                }
                else if (CanhBao.Contains("Tồn kho quá nhiều"))
                {
                    return "purple";
                }

                else if (CanhBao.Contains("Sắp hết tồn kho"))
                {

                    return "#ff9800";
                }
                return string.Empty;
            }
        }

        public long VatTuBenhVienId { get; set; }
    }

    public class VatTuTonKhoCanhBaoSearchGridVoItem : GridItem
    {
        public VatTuTonKhoCanhBaoSearchGridVoItem()
        {
            Sort = new List<Sort>();
        }
        public long KhoId { get; set; }
        public string SearchString { get; set; }
        //public string VatTu { get; set; }
        public string CanhBao { get; set; }
        public long CanhBaoId { get; set; }
        public string Description { get; set; }
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
    }

    public class VatTuTonKhoCanhBaoHTML
    {
        public string TemplateVatTuTonKhoCanhBao { get; set; }
        public string TenKho { get; set; }
        public string CanhBao { get; set; }
        public string Ngay { get; set; }
    }

    public class VatTuTonKhoTongHopGridVo : GridItem
    {
        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon == 0 ? SoLuongTon.ToString() : SoLuongTon.ApplyNumber();
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public double GiaTriSoLuongTon { get; set; }
        public string GiaTriSoLuongTonFormat => ((decimal)GiaTriSoLuongTon).ApplyFormatMoneyVND();

    }

    public class VatTuTonKhoTongHopSearchGridVoItem : GridItem
    {
        public VatTuTonKhoTongHopSearchGridVoItem()
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
    }

    public class VatTuTonKhoTongHopHTML
    {
        public string TemplateVatTuTonKhoTongHop { get; set; }
        public string TenKho { get; set; }
        public string Ngay { get; set; }
        public string TotalGiaTriSoLuongTon { get; set; }
    }

    public class VatTuTonKhoNhapXuatGridVo : GridItem
    {
        public long KhoId { get; set; }
        public string TenVatTu { get; set; }
        public string Ma { get; set; }
        public long NhomVatTuId { get; set; }
        public string TenNhomVatTu { get; set; }
        public string DonViTinh { get; set; }
        public double TonDauKy { get; set; }
        public string TonDauKyDisplay => TonDauKy == 0 ? TonDauKy.ToString() : TonDauKy.ApplyNumber();
        public double NhapTrongKy { get; set; }
        public string NhapTrongKyDisplay => NhapTrongKy == 0 ? NhapTrongKy.ToString() : NhapTrongKy.ApplyNumber();
        public double XuatTrongKy { get; set; }
        public string XuatTrongKyDisplay => XuatTrongKy == 0 ? XuatTrongKy.ToString() : XuatTrongKy.ApplyNumber();
        public double TonCuoiKy { get; set; }
        public string TonCuoiKyDisplay => TonCuoiKy == 0 ? TonCuoiKy.ToString() : TonCuoiKy.ApplyNumber();
    }

    public class VatTuTonKhoNhapXuatDetailGridVo : GridItem
    {
        public int? STT { get; set; }
        public DateTime? Ngay { get; set; }
        public string NgayDisplay { get; set; }
        public string SoPhieu { get; set; }
        public double Nhap { get; set; }
        public string NhapDisplay => Nhap == 0 ? Nhap.ToString() : Nhap.ApplyNumber();
        public double Xuat { get; set; }
        public string XuatDisplay => Xuat == 0 ? Xuat.ToString() : Xuat.ApplyNumber();
        public double Ton { get; set; }
        public string TonDisplay => Ton == 0 ? Ton.ToString() : Ton.ApplyNumber();
        public bool isNhapKho { get; set; }
    }

    public class VatTuTonKhoNhapXuatSearchGridVoItem : GridItem
    {
        public VatTuTonKhoNhapXuatSearchGridVoItem()
        {
            Sort = new List<Sort>();
        }
        public long KhoId { get; set; }
        public long VatTuId { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeDate { get; set; }
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
    }

    public class VatTuTonKhoNhapXuatHTML
    {
        public string TemplateVatTuTonKhoNhapXuat { get; set; }
        public string TenKho { get; set; }
        public string Ngay { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class ChiTietVatTuTonKhoNhapXuat
    {
        public long? KhoId { get; set; }
        public long? VatTuId { get; set; }
        public string KhoDisplay { get; set; }
        public string TenVatTu { get; set; }
    }
}
