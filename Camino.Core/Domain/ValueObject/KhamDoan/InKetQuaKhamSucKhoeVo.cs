using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class InKetQuaKhamSucKhoeVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? CongTyKhamSucKhoeId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public bool? ChuaKetLuan { get; set; }
        public bool? DaKetLuan { get; set; }
        public string SearchString { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string TenNgheNghiep { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string SoChungMinhThu { get; set; }
        public string TenDanToc { get; set; }
        public string TenTinhThanh { get; set; }
        public string NhomDoiTuongKhamSucKhoe { get; set; }
        public int? DichVuDaThucHien { get; set; }
        public int? TongDichVu { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public EnumTrangThaiYeuCauTiepNhan? TrangThaiYeuCauTiepNhan { get; set; }
        public string KSKKetLuanDisplay => TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat ? "Đã kết luận" : "Chưa kết luận";
        public int? TinhTrang => TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat ? 1 : 0;
        public bool? LaHopDongDaKetLuan { get; set; }
        // 
        public string KSKKetLuanCLSDisplay => LaHopDongDaKetLuan == true ? "Rồi" : "Chưa";
        public int? TinhTrangCLS => LaHopDongDaKetLuan == true ? 1 : 0;
        // 
        public long GoiDichVuId { get; set; }
        // BVHD-3722
        public string KetQuaKhamSucKhoeData { get; set; }
        public string HighLightClass => string.IsNullOrEmpty(KetQuaKhamSucKhoeData) ? "bg-row-lightRed" : "";

        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayString { get; set; }
        public string DenNgayString { get; set; }
        public int Take { get; set; }
        public List<InKetQuaKhamSucKhoeTiepNhanVo> InKetQuaKhamSucKhoeTiepNhanVos { get; set; } = new List<InKetQuaKhamSucKhoeTiepNhanVo>();
    }
    public class InKetQuaKhamSucKhoeTiepNhanVo
    {
        public long Id { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiYeuCauTiepNhan { get; set; }
        public string KetQuaKhamSucKhoeData { get; set; }
    }
    public class KhamDoanThongTinHanhChinhInKetQuaKhamSucKhoeVo : GridItem
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string TenGioiTinh { get; set; }
        public string NgayThangNamSinh { get; set; }
        public string TenCongTy { get; set; }
        public bool Disabled { get; set; }
    }
    public class InFoBarCodeKSK 
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string TenBN { get; set; }
    }
    public class InFoBarCodeKSKVo
    {
        public string Searching { get; set; }
        public long  HopDongId { get; set; }
    }
    public class DichVuKhamDoanVo
    {
        public long? YeuCauKhamId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long PhongBenhVienId { get; set; }
    }
}
