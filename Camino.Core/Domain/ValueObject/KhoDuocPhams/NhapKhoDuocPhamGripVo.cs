using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KhoDuocPhams
{
    public class NhapKhoDuocPhamGripVo : GridItem
    {
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        //public string SoChungTu{ get; set; }
        //public Enums.EnumLoaiNhapKho LoaiNhapKho { get; set; }
        //public string TenLoaiNhapKho { get; set; }
        //public long NguoiGiaoId { get; set; }
        //public long NguoiNhapId { get; set; }
        //public string TenNguoiGiao { get; set; }
        //public string TenNguoiNhap { get; set; }
        //public bool ChuaXepViTri { get; set; }
        //public long LoaiNhapKhoId { get; set; }
        //public DateTime NgayTao { get; set; }

        //public string NgayTaoDisplay
        //{
        //    get { return NgayTao.ApplyFormatDateTime(); }
        //}
        public string SoChungTu { get; set; }
        public string SoPhieu { get; set; }

        public long NguoiNhapId { get; set; }
        public string TenNguoiNhap { get; set; }

        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public string LoaiNguoiGiaoDisplay
        {
            get { return LoaiNguoiGiao.GetDescription(); }
        }

        public long? NguoiGiaoId { get; set; }
        public string TenNguoiGiao { get; set; }

        public DateTime? NgayNhap { get; set; }
        public string NgayNhapDisplay
        {
            get { return NgayNhap != null ? (NgayNhap ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }

        public bool? DuocKeToanDuyet { get; set; }
        public string TinhTrangDisplay
        {
            get { return DuocKeToanDuyet != null ? (DuocKeToanDuyet == true ? "Đã duyệt" : "Từ chối duyệt") : "Đang chờ duyệt"; }
        }

        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay
        {
            get { return NgayDuyet != null ? (NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay
        {
            get { return NgayHoaDon != null ? (NgayHoaDon ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }

        //BVHD-3926
        public string TenNhaCungCap { get; set; }

        public List<DataYeuCauNhapKhoDuocPhamChiTiet> DataYeuCauNhapKhoDuocPhamChiTiets { get; set; } = new List<DataYeuCauNhapKhoDuocPhamChiTiet>();
    }

    public class DataYeuCauNhapKhoDuocPhamChiTiet
    {
        public long Id { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long HopDongThauDuocPhamId { get; set; }
    }

    public class NhapKhoDuocPhamSearch
    {
        public bool DangChoDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeNhap { get; set; }
        public RangeDate RangeDuyet { get; set; }

        //BVHD-3926
        public RangeDate RangeHoaDon { get; set; }
    }
    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class NhomThuocTreeViewVo : LookupItemVo
    {
        public NhomThuocTreeViewVo()
        {
            Items = new List<NhomThuocTreeViewVo>();
        }

        //public bool IsDisabled { get; set; }
        public int Level { get; set; }
        public long? ParentId { get; set; }
        public List<NhomThuocTreeViewVo> Items { get; set; }
    }

    public class YeuCauNhapKhoDuocPhamChiTietData
    {
        public string Ten { get; set; }
        public string TenNguoiNhap { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDate();
        public double SLTheoChungTu { get; set; }
        public double SLThucNhap => SLTheoChungTu;
        public decimal DonGia { get; set; }
        public decimal ThanhTienTruocVAT { get; set; }
        public decimal ThanhTienSauVAT { get; set; }
        public decimal? ThueVatLamTron { get; set; }

        public int VAT { get; set; }
        public DateTime NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap.ApplyFormatNgayThangNam();
        public string NCC { get; set; }
        public string TheoSoHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon?.ApplyFormatDate();
        public string KhoNhap { get; set; }
        public long? KhoNhapSauDuyetId { get; set; }
        public string NguoiNhan { get; set; }
        public string SoPhieu { get; set; }
        public string KyHieuHoaDon { get; set; }
        public string KyHieuHoaDonHienThi => TheoSoHoaDon + "/" + KyHieuHoaDon;

    }

    public class YeuCauNhapKhoDuocPhamtData
    {
        public string NgayNhapKho { get; set; }
        public string LoaiBHYT { get; set; }
        public decimal TienHangDecimal { get; set; }
        public string TienHang => TienHangDecimal.ApplyFormatMoneyVND().Replace(" ₫", "");
        public string DuocPhamHoacVatTus { get; set; }
        public string VAT { get; set; }
        public decimal ThueVATDecimal { get; set; }
        public string ThueVAT => ThueVATDecimal.ApplyFormatMoneyVND().Replace(" ₫", "");
        public string ChietKhau { get; set; }
        public decimal GiaTriThanhToanDecimal => TienHangDecimal + ThueVATDecimal;
        public string GiaTriThanhToan => GiaTriThanhToanDecimal.ApplyFormatMoneyVND().Replace(" ₫", "");
        public string TongTienChu => ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(GiaTriThanhToanDecimal);
        public string NguoiLap { get; set; }
        public string NguoiGiaoHang { get; set; }
        public string ThuKho { get; set; }
        public string KeToanKho { get; set; }
        public string KeToanTruong { get; set; }
        public string NCC { get; set; }
        public string SoHoaDon { get; set; }
        public string SoPhieu { get; set; }
        public string NgayHoaDon { get; set; }
        public string KhoNhap { get; set; }
        public string NguoiNhan { get; set; }
    }
}
