using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan
{
    public class DanhSachBenhNhanChoThuNganGridVo : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string HoTenRemoveDiacritics => HoTen.RemoveDiacritics();
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhStr => GioiTinh.GetDescription();
        public string DienThoai { get; set; }
        public string DienThoaiStr { get; set; }
        public string DoiTuong { get; set; }
        public decimal SoTienBNPhaiTT =>
            (DanhSachChiPhiDichVu?.ChiPhiDichVuKhamBenh?.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiDichVuKyThuat?.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiDichVuGiuong?.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiDuocPham?.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiVatTu?.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiToaThuoc?.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiGoiDichVu?.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum() ?? 0);
        public decimal SoTienBNDaTT =>
            (DanhSachChiPhiDichVu?.ChiPhiDichVuKhamBenh?.Select(o => o.DaThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiDichVuKyThuat?.Select(o => o.DaThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiDichVuGiuong?.Select(o => o.DaThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiDuocPham?.Select(o => o.DaThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiVatTu?.Select(o => o.DaThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiToaThuoc?.Select(o => o.DaThanhToan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachChiPhiDichVu?.ChiPhiGoiDichVu?.Select(o => o.DaThanhToan).DefaultIfEmpty(0).Sum() ?? 0);
        public bool KiemTraChonThanhToan { get; set; }


        //update 18/07/2020
        public decimal SoTienTamUng { get; set; }
        public decimal SoTienDaThu { get; set; }
        public decimal SoTienDuTaiKhoan { get; set; }

        public bool ChuaThu { get; set; }
        public bool CongNo { get; set; }
        public bool HoanUng { get; set; }

        public Enums.TrangThaiThuNgan TrangThai => ChuaThu ? Enums.TrangThaiThuNgan.ChuaThu : (CongNo ? Enums.TrangThaiThuNgan.CongNo : (HoanUng ? Enums.TrangThaiThuNgan.HoanUng : Enums.TrangThaiThuNgan.DaThu));
        public DanhSachChiPhiDichVuVo DanhSachChiPhiDichVu { get; set; }

        public string SearchString { get; set; }

        public string ThoiDiemTiepNhanDisplay { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }

        public string ToDate { get; set; }
        public string FromDate { get; set; }

        public bool YeuCauNhapVien { get; set; }
    }

    public class DanhSachChiPhiDichVuVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public IEnumerable<ChiPhiDichVuVo> ChiPhiDichVuKhamBenh { get; set; }
        public IEnumerable<ChiPhiDichVuVo> ChiPhiDichVuKyThuat { get; set; }
        public IEnumerable<ChiPhiDichVuVo> ChiPhiDichVuGiuong { get; set; }
        public IEnumerable<ChiPhiDichVuVo> ChiPhiDuocPham { get; set; }
        public IEnumerable<ChiPhiDichVuVo> ChiPhiVatTu { get; set; }
        public IEnumerable<ChiPhiDichVuVo> ChiPhiToaThuoc { get; set; }
        public IEnumerable<ChiPhiDichVuVo> ChiPhiGoiDichVu { get; set; }
    }
    public class ChiPhiDichVuVo
    {
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public decimal SoTienMG { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal TongCongNo { get; set; }
        public decimal BHYTThanhToan => DuocHuongBHYT ? (decimal)(Soluong * (double)DonGiaBHYTThanhToan) : 0;
        public decimal BNConPhaiThanhToan => ThanhTien - BHYTThanhToan - TongCongNo - SoTienMG - DaThanhToan;
        public decimal ThanhTien => KhongTinhPhi == true ? 0 : (decimal)(Soluong * (double)DonGia);
        public bool YeuCauNhapVien { get; set; }
    }
}