using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Core.Domain.ValueObject.BaoCaoTheKhos
{
    public class BaoCaoTheKhoGridVo : GridItem
    {
        public BaoCaoTheKhoGridVo()
        {
            BaoCaoTheKhoChiTiets = new List<BaoCaoTheKhoChiTietGridVo>();
        }
        public long? KhoId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public double? TongSLTonDauKy { get; set; }
        public double? TongSLNhap { get; set; }
        public double? TongSLXuat { get; set; }
        public double? TongSLTon { get; set; }
        public DateTime NgayThang { get; set; }
        public string NgayThangDisplay => NgayThang.ApplyFormatDate();
        public List<BaoCaoTheKhoChiTietGridVo> BaoCaoTheKhoChiTiets { get; set; }
    }

    public class BaoCaoTheKhoThongTinXuatKhoDeNhapVe
    {
        public long XuatKhoId { get; set; }
        public long KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long HopDongThauId { get; set; }
        public string SoHoaDon { get; set; }
        public bool KhoKhacHoanTra { get; set; }
    }

    public class BaoCaoTheKhoChiTietGridVo : GridItem
    {
        public string Ten { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public string DVT { get; set; }
        public long? NhapTuXuatKhoId { get; set; }
        public DateTime NgayThangCT { get; set; }
        public string SCTNhap { get; set; }
        public string SCTXuat { get; set; }
        public bool KhoKhacHoanTra { get; set; }
        public string DienGiai => BenhNhanTraLai == true
            ? $"{TenKho} nhận hoàn trả từ {(!string.IsNullOrEmpty(KhoaLinh) ? KhoaLinh : $"bệnh nhân {(!string.IsNullOrEmpty(HoTenBenhNhan) ? (HoTenBenhNhan + " - mã TN: " + MaTiepNhan) : "")}")}"
            : SLXuat.GetValueOrDefault() > 0
                ? ((LoaiXuatKho == Enums.XuatKhoDuocPham.XuatChoBenhNhan || LoaiXuatKhoVatTu == Enums.EnumLoaiXuatKho.XuatChoBenhNhan) 
                    ? (!string.IsNullOrEmpty(KhoaLinh) ? $"{TenKho} xuất cho bệnh nhân {KhoaLinh}" : (!string.IsNullOrEmpty(HoTenBenhNhan) ? $"{TenKho} xuất cho bệnh nhân {HoTenBenhNhan} - mã TN: {MaTiepNhan}" : $"{TenKho} xuất cho bệnh nhân"))
                    : (!string.IsNullOrEmpty(XuatQuaKho) ? $"{TenKho} xuất nội bộ {XuatQuaKho}" : $"{TenKho} xuất khác"))
                : (KhoKhacHoanTra ? $"{TenKho} nhận hoàn trả từ {NhapTuKho}" : (NhapTuNhaCungCap && !string.IsNullOrEmpty(SoHoaDon) ? $"Mua từ công ty: {TenNhaThau} - {SoHoaDon}" : (!string.IsNullOrEmpty(NhapTuKho) ? $"{TenKho} nhập từ kho {NhapTuKho}" : $"Nhập từ hợp đồng thầu {SoHopDongThau} {TenNhaThau}")));
                //!string.IsNullOrEmpty(NhapTuKho) ? $"{TenKho} nhập từ kho {NhapTuKho}" : (NhapTuNhaCungCap ? $"Nhập từ nhà cung cấp {TenNhaThau}" : $"Nhập từ hợp đồng thầu {SoHopDongThau} {TenNhaThau}");
        public double? SLTonDauKy { get; set; }
        public double? SLNhap { get; set; }
        public double? SLXuat { get; set; }
        public double? SLTon { get; set; }
        public decimal DonGia { get; set; }
        public DateTime NgayThang { get; set; }
        public string NgayThangDisplay => NgayThang.ApplyFormatDate();
        //nhap
        public string NhapTuKho { get; set; }
        public string TenNhaThau { get; set; }
        public long HopDongThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoHoaDon { get; set; }
        public bool NhapTuNhaCungCap { get; set; }
        //Xuat
        public string XuatQuaKho { get; set; }
        public Enums.XuatKhoDuocPham LoaiXuatKho { get; set; }
        public Enums.EnumLoaiXuatKho LoaiXuatKhoVatTu { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public bool BenhNhanTraLai { get; set; }
        public List<long> DonThuocThanhToanIds { get; set; }
        public long XuatKhoDuocPhamChiTietId { get; set; }
        public string KhoaLinh { get; set; }
    }
}
