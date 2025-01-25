using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camino.Core.Domain.ValueObject
{
    public class BaoCaoDoanhThuTheoNhomDichVuDataPhieuThu
    {
        public long PhieuThuId { get; set; }
        public long PhieuChiId { get; set; }
        public DateTime NgayThu { get; set; }
        public bool LaPhieuHuy { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaNB { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string SoBenhAn { get; set; }
        
        public List<BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi> DataPhieuChis { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public string NguoiGioiThieu { get; set; }
        public bool GoiDichVu { get; set; }
        public long? HinhThucDenId { get; set; }
        public long? BenhNhanId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
    }

    public class BaoCaoDoanhThuTheoNhomDichVuDataPhieuChi
    {
        public long Id { get; set; }
        
        public DateTime NgayChi { get; set; }
        
        public decimal? TienChiPhi { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }
        
        public long? YeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public long? DonVTYTThanhToanChiTietId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }

        public decimal? Gia { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public decimal? SoTienMienGiam { get; set; }

        //public bool? DaHuy { get; set; }
        //public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        //public string SoPhieuHienThi { get; set; }
        //public decimal? Gia { get; set; }
        //public double? SoLuong { get; set; }
        //public decimal? DonGiaBaoHiem { get; set; }
        //public int? MucHuongBaoHiem { get; set; }
        //public int? TiLeBaoHiemThanhToan { get; set; }

        //public decimal? SoTienMienGiam { get; set; }        
        //public long? PhieuThanhToanChiPhiId { get; set; }
    }
    public class BaoCaoDoanhThuTheoNhomDichVuDataDichVu
    {
        public long Id { get; set; }        
        public string TenDichVu { get; set; }
        public bool YeuCauKhamBenh { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public bool YeuCauDichVuKyThuat { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public bool YeuCauGiuong { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }
        public bool YeuCauDuocPham { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool DonThuocBHYT { get; set; }
        public bool YeuCauVatTu { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool YeuCauTruyenMau { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? NoiThucHienKhamBenhId { get; set; }
        public long? NoiThucHienPTTTId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public decimal? DonGiaNhapDuocPham { get; set; }
        public decimal? DonGiaNhapVatTu { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public long? MauVaChePhamId { get; set; }
        public string BacSiKham { get; set; }
    }
    public class BaoCaoDoanhThuTheoNhomDichVuDataVo
    {
        public long Id { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string SoBenhAn { get; set; }
        public string TenDichVu { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public double Soluong { get; set; }
        public decimal? Gia { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal ThanhTien => (YeuCauGoiDichVuId != null ? DonGiaSauChietKhau.GetValueOrDefault() : Gia.GetValueOrDefault()) * (decimal) Soluong - SoTienMienGiam.GetValueOrDefault();
        public List<BaoCaoDoanhThuTheoNhomDichVuDataChiVo> DataChis { get; set; }
        public string Nhom { get; set; }
        public string NguoiGioiThieu { get; set; }

        public bool YeuCauKhamBenh { get; set; }
        public bool YeuCauDichVuKyThuat { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public bool YeuCauGiuong { get; set; }
        public bool YeuCauDuocPham { get; set; }
        public bool DonThuocBHYT { get; set; }
        public bool YeuCauVatTu { get; set; }
        public bool YeuCauTruyenMau { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? NoiThucHienKhamBenhId { get; set; }
        public long? NoiThucHienPTTTId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }

        public DateTime? ThoiDiemTinhHoanThanh => ThoiDiemHoanThanh != null
            ? ThoiDiemHoanThanh
            : (YeuCauDichVuKyThuat && LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.Khac
                ? DataChis.OrderBy(o => o.NgayChi).LastOrDefault(o => o.DaHuy != true)?.NgayChi
                : null);
    }

    public class BaoCaoDoanhThuTheoNhomDichVuDataChiVo
    {
        public long Id { get; set; }
        public bool? DaHuy { get; set; }
        public DateTime NgayChi { get; set; }
    }

    public class BaoCaoDoanhThuTheoNhomDichVuDataDichVuBSGioiThieu
    {
        public long Id { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public int SoLan { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
    }

    public class BaoCaoDoanhThuTheoNhomDichVuGridVo : GridItem
    {
        public int STT { get; set; }
        public string Nhom { get; set; }
        public long? KhoaPhongId { get; set; }
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public string HoVaTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string SoBenhAn { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ToString("HH:mm dd/MM/yyyy");
        public string NguoiGioiThieu { get; set; }

        public decimal? KhamBenh { get; set; }
        public decimal? XetNghiem { get; set; }
        public decimal? NoiSoi { get; set; }
        public decimal? NoiSoiTMH { get; set; }
        public decimal? SieuAm { get; set; }
        public decimal? XQuang { get; set; }
        public decimal? CTScan { get; set; }
        public decimal? MRI { get; set; }
        public decimal? DienTimDienNao { get; set; }
        public decimal? TDCNDoLoangXuong { get; set; }
        public decimal? ThuThuat { get; set; }
        public decimal? PhauThuat { get; set; }
        public decimal? NgayGiuong { get; set; }
        public decimal? DVKhac { get; set; }
        public decimal? Thuoc { get; set; }
        public decimal? VTYT { get; set; }
        public bool ThuocVTYTThamMy { get; set; }
        public decimal? TongCong { get; set; }
        public decimal? GiaNhapKho { get; set; }
        public decimal? GiaNiemYet { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal? BHYTChiTra { get; set; }
        public decimal HeSo { get; set; }
    }

    public class TotalBaoCaoDoanhThuTheoNhomDichVuGridVo : GridItem
    {
        public decimal? TotalKhamBenh { get; set; }
        public decimal? TotalXetNghiem { get; set; }
        public decimal? TotalNoiSoi { get; set; }
        public decimal? TotalNoiSoiTMH { get; set; }
        public decimal? TotalSieuAm { get; set; }
        public decimal? TotalXQuang { get; set; }
        public decimal? TotalCTScan { get; set; }
        public decimal? TotalMRI { get; set; }
        public decimal? TotalDienTimDienNao { get; set; }
        public decimal? TotalTDCNDoLoangXuong { get; set; }
        public decimal? TotalThuThuat { get; set; }
        public decimal? TotalPhauThuat { get; set; }
        public decimal? TotalNgayGiuong { get; set; }
        public decimal? TotalDVKhac { get; set; }
        public decimal? TotalThuoc { get; set; }
        public decimal? TotalVTYT { get; set; }
        public decimal? TotalTongCong { get; set; }
    }
}
