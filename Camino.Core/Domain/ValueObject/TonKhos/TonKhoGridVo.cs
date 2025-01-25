using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.TonKhos
{
    public class TonKhoGridVo : GridItem
    {
        public string DuocPham {get;set; }
        public string HoatChat { get; set; }
        public string DonViTinhName { get; set; }
        public double SoLuongTon { get; set; }
        public int? TonToiThieu { get; set; }
        public int? TonToiDa { get; set; }
        public string CanhBao {
            get
            {
                if (SoLuongTon<=0.00001)
                {
                    return "Hết tồn kho";
                }
                else if(TonToiDa !=null && SoLuongTon > TonToiDa)
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
        public string MauCanhBao {
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

        public long DuocPhamBenhVienId { get; set; }
        public string SoDangKy { get; set; }
        public string PhanLoai { get; set; }
        public string HamLuong { get; set; }

        //BVHD-3912
        public string MaDuocPham { get; set; }
    }
    public class TonKhoTatCaGridVo : GridItem
    {
        public string DuocPham { get; set; }
        public string HoatChat { get; set; }
        public string PhanLoai { get; set; }
        public string HamLuong { get; set; }
        public string DonViTinhName { get; set; }
        public double SoLuongTon { get; set; }
        public decimal GiaTriSoLuongTon { get; set; }
        public string GiaTriSoLuongTonFormat => GiaTriSoLuongTon.ApplyFormatMoneyVND();
        public string SoDangKy { get; set; }


        //BVHD-3912
        public string MaDuocPham { get; set; }
    }
    public class TonKhoGridVoItem : GridItem
    {
        public long KhoId { get; set; }
        public string Description { get; set; }
        public string CanhBao { get; set; }
        public long CanhBaoId { get; set; }
        public string searchString { get; set; }
        //public string LoaiCanhBao { get; set; }

    }
    public class DuocPhamSapHetHanGridVo : GridItem
    {
        public string TenKho { get; set; }
        public long? VitriId { get; set; }
        public long? KhoId { get; set; }
        public string TenDuocPham { get; set; }
        public string PhanLoai { get; set; }
        public string HamLuong { get; set; }
        public string TenHoatChat { get; set; }
        public string DonViTinh { get; set; }
        public string ViTri { get; set; }
        public DateTime NgayHetHan { get; set; }
        public int SoNgayTruocKhiHetHan { get; set; }
        public string NgayHetHanHienThi { get; set; }
        public double SoLuongTon { get; set; }
        public string MaDuocPham { get; set; }
        public string SoLo { get; set; }
        public decimal DonGiaNhap { get; set; }
        public long NhapKhoDuocPhamId { get; set; }
        public long DuocPhamId { get; set; }
        public double ThanhTien => SoLuongTon != 0 && DonGiaNhap != 0 ? (SoLuongTon * Convert.ToDouble(DonGiaNhap)).MathRoundNumber(2) : 0;

        public string HoatChat { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public Enums.LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public bool LaDuocPhamHayVatTu { get; set; }
    }
    public class DuocPhamSapHetHanSearchGridVoItem : GridItem
    {
        public long? KhoId { get; set; }
        public string DuocPham { get; set; }
    }
    public class DataVaLueHTML
    {
        public string TemplateDuocPham { get; set; }
        public string TemplateVatTu { get; set; }
        public string Ngay { get; set; }
    }
    public class DataDuocPhamCanhBaoHTML
    {
        public string TemplateDuocPhamCanhBao { get; set; }
    }
    public class DataTitleCanhBaoHTML
    {
        public string TemplateDuocPhamCanhBao { get; set; }
        public string TenKho { get; set; }
        public string LoaiCanhBao { get; set; }
        public string NgayNow { get; set; }
        public string ThangNow { get; set; }
        public string NamNow { get; set; }

    }
    public class DataTitleTonKhoHTML
    {
        public string TemplateDuocPhamTonKho { get; set; }
        public string TenKho { get; set; }
        public string NgayNow { get; set; }
        public string ThangNow { get; set; }
        public string NamNow { get; set; }
        public string TotalGiaTriSoLuongTon { get; set; }
    }

    public class DataTitleXuatNhapTonKhoHTML
    {
        public string TemplateDuocPhamTonKho { get; set; }
        public string TenKho { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NgayNow { get; set; }
        public string ThangNow { get; set; }
        public string NamNow { get; set; }

    }
}
