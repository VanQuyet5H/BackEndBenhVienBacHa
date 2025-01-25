using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong
{
    public class NoiGioiThieuHopDongQueryInfo : QueryInfo
    {
        public string TimKiemSearch { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public bool selectedTabGioiThieu { get; set; }
        public bool selectedTabHoaHong { get; set; }
    }
    public class CauHinhHeSoTheoNoiGioiThieuHoaHongGridVo : GridItem
    {
        public string Ten { get; set; }
        public string DonVi { get; set; }
        public string Sdt { get; set; }
        public string MoTa { get; set; }
    }
    public class NoigioiThieuHopDongGridVo : GridItem
    {
        public string Ten { get; set; }

        public DateTime NgayBatDau { get; set; }
        public string NgayBatDauDisplay => NgayBatDau.ApplyFormatDate();
        public DateTime? NgayKetThuc { get; set; }
        public string NgayKetThucDisplay => NgayKetThuc != null ? NgayKetThuc.GetValueOrDefault().ApplyFormatDate() : "";
    }
    public class LookupItemCauHinhHeSoTheoNoiGioiThieuHoaHongVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }
        public string DonVi { get; set; }
        public string SoDienThoai { get; set; }

        public string MoTa { get; set; }
    }

    public class NoiGioiThieuHopDongVo
    {
        public long Id { get; set; }

        public string Ten { get; set; }

        public bool ThemhayCapNhat { get; set; }

    }

    public class DichVuAndThuocAndVTYTTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public string TenKhoa { get; set; }
    }
    public class ThongTinGiaNoiGioiThieuVo
    {
        public long? NhomGiaId { get; set; }
        public string TenNhomGia { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public decimal? DonGia { get; set; }
    }
    public class KetQuaThemVo :GridItem
    {
        public long NoiGioiThieuHopDongId { get; set; }
    }
    public class NoiGioiThieuVo : GridItem
    {
        public long NoiGioiThieuId { get; set; }
    }
    public class CauHinhHeSoTheoThoiGianHoaHong : GridItem
    {
        public CauHinhHeSoTheoThoiGianHoaHong()
        {
            ThongTinCauHinhHeSoTheoNoiGtHoaHongs = new List<ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo>();
        }
        public string Ten { get; set; }
        public string TenNoiGioiThieuHopDong { get; set; }
        public long NoiGioiThieuId { get; set; }
        public string Donvi { get; set; }
        public string SoDienThoai { get; set; }
        public string MoTa { get; set; }
        public long NoiGioiThieuHopDongId { get; set; }
        public long ChonLoaiDichVuId { get; set; }

        public List<ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo> ThongTinCauHinhHeSoTheoNoiGtHoaHongs { get; set; }
    }
    public class ThongTinCauHinhHeSoTheoNoiGtHoaHongGridVo : GridItem
    {

        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }

        public long? TenNhomGiaId  { get; set; }
        public string TenNhomGia { get; set; }

        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }

        public LoaiGiaNoiGioiThieuHopDong? NhomGiaThuocId { get; set; }
        public LoaiGiaNoiGioiThieuHopDong? NhomGiaVTYTId { get; set; }

        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }

        public string GhiChu { get; set; }
        public string TenDichVu { get; set; }
        public string MaDichVu { get; set; }

        public string Nhom { get; set; }
        public decimal? DonGia { get; set; }

        public bool LaDichVuKham { get; set; }
        public bool LaDichVuKyThuat { get; set; }
        public bool LaDichVuGiuong { get; set; }
        public bool LaDuocPham { get; set; }
        public bool LaVatTu { get; set; }
        public bool? IsEdit { get; set; }



        public decimal? DonGiaNGTTuLan1 { get; set; }
        public decimal? HeSoLan1 { get; set; }
        public decimal? DonGiaNGTTuLan2 { get; set; }
        public decimal? HeSoLan2 { get; set; }
        public decimal? DonGiaNGTTuLan3 { get; set; }
        public decimal? HeSoLan3 { get; set; }
        public decimal? HeSo { get; set; }
    }

    public class DeleteNoiGioiThieuHopDongVo : GridItem
    {
        public bool LaDichVuKham { get; set; }
        public bool LaDichVuKyThuat { get; set; }
        public bool LaDichVuGiuong { get; set; }
        public bool LaDuocPham { get; set; }
        public bool LaVatTu { get; set; }
    }

    public class LookupItemCauHinhHeSoTheoNoiGtHoaHongVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public DateTime ThoiGian { get; set; }
    }
    public class ImportNoiGioiThieuDichVu
    {
       

        public string Error { get; set; }
        public bool IsError { get; set; }
        
        public string LaDichVu { get; set; }
        
        public string Ma { get; set; }
      
        public string TenDichVu { get; set; }
        public string TenDV{ get; set; }

        public string NhomGiaDichVu { get; set; }

      
        public string DonGia { get; set; }
      
        public string DonGiaNGTTuLan1 { get; set; }
       
        public string HeSoLan1 { get; set; }
      
        public string DonGiaNGTTuLan2 { get; set; }
       
        public string HeSoLan2 { get; set; }
      
        public string DonGiaNGTTuLan3 { get; set; }
       
        public string HeSoLan3 { get; set; }
        
        public string GhiChu { get; set; }

    }
    public class CauHinhNoiGioiThieuDichVu { 
        public CauHinhNoiGioiThieuDichVu() { 
        }
       
        public int LaDichVu { get; set; }
       
        public string Ma { get; set; }
       
        public string TenDichVu { get; set; }
       
        public string NhomGiaDichVu { get; set; }

       
        public decimal DonGia { get; set; }
       
        public decimal DonGiaNGTTuLan1 { get; set; }
       
        public decimal HeSoLan1 { get; set; }
       
        public decimal? DonGiaNGTTuLan2 { get; set; }
       
        public decimal? HeSoLan2 { get; set; }
       
        public decimal? DonGiaNGTTuLan3 { get; set; }
       
        public decimal? HeSoLan3 { get; set; }
       
        public string Ghichu { get; set; }
    }
    
    public class DSChuaTaoImport
    {
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public long NoiGioiThieuId { get; set; }
        public long NoiGioiThieuHopDongId { get; set; }
    }


   
    public class CauHinhNoiGioiThieuDPVTYTVo
    {
        public  CauHinhNoiGioiThieuDPVTYTVo()
        {
        }
        public int LaDichVu { get; set; }

       
        public string Ma { get; set; }

      
        public string Ten { get; set; }

        
        public string NhomGia { get; set; }

       
        public decimal? HeSo { get; set; }
        
        public string Ghichu { get; set; }
    }

    public class ImportNoiGioiThieuDuocPhamVTYT
    {


        public string Error { get; set; }
        public bool IsError { get; set; }

        public string LaDichVu { get; set; }


        public string Ma { get; set; }


        public string Ten { get; set; }
        public string TenDV { get; set; }


        public string NhomGia { get; set; }


        public string HeSo { get; set; }

        public string GhiChu { get; set; }

    }

    public class CauHinhHoaHongDichVuVo
    {
        public CauHinhHoaHongDichVuVo()
        {
        }
        
        public string LaDichVu { get; set; }

      
        public string Ma { get; set; }

       
        public string Ten { get; set; }

        
        public string NhomGia { get; set; }

       
        public decimal DonGia { get; set; }

       
        public string LoaiHoaHong { get; set; }

        
        public decimal DonGiaHoaHong { get; set; }

        
        public int ApDungHoaHongTuLan { get; set; }

        
        public int ApDungHoaHongDenLan { get; set; }

        
        public string Ghichu { get; set; }

        public string ApDungHoaHongTuLanDisplay => Convert.ToDecimal(ApDungHoaHongTuLan).ApplyFormatTien();


        public string ApDungHoaHongDenLanDisplay => ApDungHoaHongDenLan != null && ApDungHoaHongDenLan  != 0 ? Convert.ToDecimal(ApDungHoaHongDenLan).ApplyFormatTien() :"";

    }


    public class ImportHoaHongDichVu
    {


        public string Error { get; set; }
        public bool IsError { get; set; }

        public string LaDichVu { get; set; }


        public string Ma { get; set; }


        public string Ten { get; set; }
        public string TenDV { get; set; }


        public string NhomGia { get; set; }


        public string DonGia { get; set; }

        public string LoaiHoaHong { get; set; }
        public string DonGiaHoaHong { get; set; }
        public string ApDungHoaHongTuLan { get; set; }
        public string ApDungHoaHongDenLan { get; set; }

        public string GhiChu { get; set; }

    }

    public class CauHinhHoaHongDuocPhamVTYTVo
    {
        public CauHinhHoaHongDuocPhamVTYTVo()
        {
        }
        public int LaDichVu { get; set; }


        public string Ma { get; set; }


        public string Ten { get; set; }


        public decimal? DonGiaHoaHong { get; set; }

        public string Ghichu { get; set; }
    }
    public class ImportHoaHongDPVTYT
    {


        public string Error { get; set; }
        public bool IsError { get; set; }

        public string LaDichVu { get; set; }


        public string Ma { get; set; }


        public string Ten { get; set; }
        public string TenDV { get; set; }


        public string DonGiaHoaHong { get; set; }

        public string GhiChu { get; set; }

    }

    public class ExportDanhSachCauHinhNoiGioiThieuDichVuQueryInfo 
    {
        public long? NoiGioiThieuId { get; set; }

        public long? NoiGioiThieuHopDongId { get; set; }
    }

}
