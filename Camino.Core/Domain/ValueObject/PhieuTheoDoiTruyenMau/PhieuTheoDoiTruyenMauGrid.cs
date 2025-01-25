using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau
{
    public class PhieuTheoDoiTruyenMauGrid : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuTheoDoiTruyenMauGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuTheoDoiTruyenMauGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class ThongTinDefaultPhieuTheoDoiTruyenMauCreate
    {
        public string TenNhanVien { get; set; }
        public string NgayThucHien { get; set; }
        public string ChanDoan { get; set; }
    }
    public class XacNhanInPhieuTheoDoiTruyenMau
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
    }
    public class InPhieuTheoDoiTruyenMau
    {
        public int? LanTruyenMauThu  { get;set; }
        public string DinhNhomDonViMauChePhamMau { get; set; }
        public string DinhNhomMauNguoiNhan { get; set; }
        public string PhanUngHoaHopTaiGiuong { get; set; }
        public DateTime? BatDauTruyenHoi { get; set; }
        public string BatDauTruyenHoiStringUTC { get; set; }
        public string NgungTruyenHoiStringUTC { get; set; }
        public DateTime? NgungTruyenHoi { get; set; }
        public int? SLMauTruyenThucTe { get; set; }
        public string BSDieuTri { get; set; }
        public string DDTruyenMau { get; set; }
        public long DDTruyenMauId { get; set; }
        public string ChanDoan { get; set; }
        public long MaDonViMauTruyenId { get; set; }
        public List<PhieuTheoDoiTruyenMauGridVo> DachSachTruyenMauArr { get; set; }

    }
    public class PhieuTheoDoiTruyenMauGridVo : GridItem
    {
        public int? ThoiGian { get; set; }
        public int? TocDoTruyen { get; set; }
        public string MauSacDaNiemMac { get; set; }
        public int? NhipTho { get; set; }
        public double? ThanNhiet { get; set; }
        public string HuyetAp { get; set; }

        public int? Mach { get; set; }
        public string DienBienKhac { get; set; }
        public string HuyetAp1 { get; set; }
        public string HuyetAp2 { get; set; }

    }
}
