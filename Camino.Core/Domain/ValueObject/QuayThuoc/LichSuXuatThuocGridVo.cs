using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.QuayThuoc
{

    public class DanhSachLichSuXuatThuocGridVo : GridItem
    {
        public LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public string SoChungTu { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? LoaiGioiTinh { get; set; }
        public string GioiTinh => LoaiGioiTinh?.GetDescription();
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string DoiTuong { get; set; }

        public DateTime NgayXuatThuoc { get; set; }
        public string NgayXuatThuocDisplay => NgayXuatThuoc.ApplyFormatDateTimeSACH();
    }

    public class LichSuXuatThuocGridVo : GridItem
    {
        public string FromDate { get; set; }
        public long? TaiKhoanBenhNhanThuId { get; set; }
        public long? TaiKhoanBenhNhanThuBHYT { get; set; }
        public string ToDate { get; set; }
        public string MaTN { get; set; }
        public string MaTNemoveDiacritics => MaTN.RemoveDiacritics();
        public string MaBN { get; set; }
        public string MaBNemoveDiacritics => MaBN.RemoveDiacritics();
        public string SoDon { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public string MaBNRemoveDiacritics => MaBN.RemoveDiacritics();
        public string HoTen { get; set; }
        public string HoTenRemoveDiacritics => HoTen.RemoveDiacritics();
        public string NgayThuStr { get; set; }
        public DateTime NgayThu { get; set; }

        public string NgayThuStRemoveDiacritics => NgayThuStr.RemoveDiacritics();
        public DateTime? ThoiDiemCapPhatThuoc { get; set; }
        public string ThoiDiemCapPhatThuocString => ThoiDiemCapPhatThuoc != null ? ThoiDiemCapPhatThuoc.Value.ApplyFormatDateTimeSACH() : "";
        public string ThoiDiemCapPhatThuocRemoveDiacritics => ThoiDiemCapPhatThuoc.ToString().RemoveDiacritics();
        public decimal SoTienVatTu { get; set; }
        public decimal SoTienDuocPham { get; set; }
        public decimal SoTienThu => SoTienVatTu + SoTienDuocPham;
        public string SoTienThuString => SoTienThu.ApplyFormatMoneyVND();
        public TrangThaiThanhToan TrangThai { get; set; }
        public List<DateTime?> llistdonthuoctt { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? CongNo { get; set; }
        public string DoiTuong { get; set; }
        public int? NamSinh { get; set; }
        public string GioiTinhHienThi { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }

        public string SoChungTuDuocPham { get; set; }
        public string SoChungTuVatTu { get; set; }
        public List<string> SoChungTuDuocPhams { get; set; }
        public List<string> SoChungTuVatTus { get; set; }
        public List<DateTime?> ThoiDiemCapThuocs { get; set; }
        public List<DateTime?> ThoiDiemCapVatTus { get; set; }

        public List<DonVTYTThanhToanChiTiet> donVatTu { get; set; }
        public List<DonThuocThanhToanChiTiet> donDuocPham { get; set; }
        public List<DonVTYTThanhToan> donVatTuTT { get; set; }
        public List<DonThuocThanhToan> donThuocTT { get; set; }   
    }

    public class ThongTinHuyPhieuXuatTrongNgayVo
    {
        public long TaiKhoanBenhNhanThuId { get; set; }
        public bool? ThuHoiPhieu { get; set; }
        public int? NguoiThuHoiId { get; set; }
        public string TenNguoiThuHoi { get; set; }
        public DateTime? ThoiGianThuHoi { get; set; }
        public string LyDo { get; set; }
    }
}
