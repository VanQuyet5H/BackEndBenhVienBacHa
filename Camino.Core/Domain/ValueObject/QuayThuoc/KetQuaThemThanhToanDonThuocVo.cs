using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.QuayThuoc
{
    public class KetQuaThemThanhToanDonThuocVo
    {
        public bool ThanhCong { get; set; }
        public long TaiKhoanBenhNhanThuId { get; set; }
        public string Error { get; set; }
        public List<DuocPhamVuotTonKho> DanhSachDuocPhamVuotTonKho { get; set; }
    }
    public class DuocPhamVuotTonKho
    {
        public long Stt { get; set; }
        public double SoLuongTonKho { get; set; }
    }

    public class XacNhanInThuocVatTu
    {
        public long PhieuXuatId { get; set; }
        public string Hosting { get; set; }       
        public LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        
    }

    public class XacNhanIn
    {
        public long TaiKhoanBenhNhanThuId { get; set; }
        public bool InPhieuThu { get; set; }
        public bool InBangKe { get; set; }
        public string Hosting { get; set; }
    }

    public class XacNhanInThuocBhyt
    {
        public long TiepNhanId { get; set; }
        public long TaiKhoanBenhNhanThuId { get; set; }
        public long TaiKhoanBenhNhanChiId { get; set; }

        public string Hosting { get; set; }
    }
    public class XacNhanInThuocBenhNhan
    {
        public XacNhanInThuocBenhNhan()
        {
            ListGridThuoc = new List<DonThuocChiTietGridVoItem>();
        }
        public long TiepNhanId { get; set; }
        public string Hosting { get; set; }
        public string LoaiDonThuoc { get; set; }
        public long YeuCauKhambenhId { get; set; }
        public bool Header { get; set; }
        public List<DonThuocChiTietGridVoItem> ListGridThuoc { get; set; }
    }

    public class ThongTinPhieuThuQuayThuoc
    {
        public ThongTinPhieuThuQuayThuoc()
        {
            DanhSachThuPhis = new List<ChiPhiThuocVatTuVo>();
        }

        public long Id { get; set; }
        public string SoPhieu { get; set; }
        public bool LaPhieuChi { get; set; }
        public string LoaiPhieu => "Phiếu thu";
        public bool? DaHuy { get; set; }
        public bool? DaHoanUng { get; set; }
        public string PhieuHoanUng { get; set; }
        public string TinhTrang => DaHuy == true ? "Đã hủy" : "Đang hiệu lực";
        public decimal SoTien => TienMat.GetValueOrDefault(0) + ChuyenKhoan.GetValueOrDefault(0) + Pos.GetValueOrDefault(0);
        public string HinhThucThanhToan { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? CongNo { get; set; }
        public decimal? TongChiPhi { get; set; }
        public decimal? BHYTThanhToan { get; set; }
        public decimal? MienGiam { get; set; }
        public decimal? BenhNhanThanhToan { get; set; }
        public decimal? TamUng { get; set; }
        public decimal? SoTienPhaiThuHoacChi { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ApplyFormatDateTimeSACH();
        public string NoiDungThu { get; set; }

        public bool ConHanHuyPhieuThu => NgayThu.Date == DateTime.Now.Date;
        public bool DaThuHoi { get; set; }
        public long? NguoiThuHoiId { get; set; }
        public string NguoiThuHoi { get; set; }
        public DateTime? NgayThuHoi { get; set; }
        public string NhanVienHuyPhieu { get; set; }
        public DateTime? NgayHuy { get; set; }
        public string NgayHuyStr => NgayHuy?.ApplyFormatDateTimeSACH();

        public List<ChiPhiThuocVatTuVo> DanhSachThuPhis { get; set; }
    }

    public class ChiPhiThuocVatTuVo : GridItem
    {
        public long? TaiKhoanBenhNhanChiId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public string Nhom { get; set; }
        public DateTime? NgayPhatSinh { get; set; }
        public string Khoa { get; set; }
        //public string Nhom => LoaiNhom.GetDescription();
        public int STT { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal ThanhTien => KhongTinhPhi == true ? 0 : (decimal)(Soluong * (double)DonGia);
        public bool KiemTraBHYTXacNhan { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public decimal BHYTThanhToan => DuocHuongBHYT ? (decimal)(Soluong * (double)DonGiaBHYTThanhToan) : 0;
        public int TLMG { get; set; }
        public decimal SoTienMG { get; set; }
        public decimal SoTienBaoHiemTuNhanChiTra { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal BNConPhaiThanhToan => ThanhTien - BHYTThanhToan - TongCongNo - SoTienMG - DaThanhToan;
        public string NoiThucHien { get; set; }
        //Biến này em dùng checkUi CheckedDefault = flalse 
        public bool CheckedDefault { get; set; }
        public bool CapNhatThanhToan { get; set; }

        public decimal TongCongNo => SoTienBaoHiemTuNhanChiTra;
        public bool DaHoanThu { get; set; }
        public bool DuocHoanThu => NgayThu != null && NgayThu.Value.Date != DateTime.Now.Date;
        public DateTime? NgayThu { get; set; }

        public string NguoiThu { get; set; }
        public string ThoiGianThuStr { get; set; }
        public int? SoPhieu { get; set; }
        public bool HuyDichVu { get; set; }
        public string DonViTinh { get; set; }

        //update 17/08/2020
        public DateTime ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhStr => ThoiDiemChiDinh.ApplyFormatDateTimeSACH();
        public decimal SoTienCongNo { get; set; }
        public int TrangThaiThanhToan { get; set; }

    }

    public class ThongTinPhieuThuQuayThuocVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string LoaiPhieu => "Phiếu thu";
        public DateTime? NgayLap { get; set; }
        public bool? DaHuy { get; set; }
        public string TinhTrang => DaHuy == true ? "Đã hủy" : "Đang hiệu lực";
        public bool? DaThuHoi { get; set; }
    }
}
