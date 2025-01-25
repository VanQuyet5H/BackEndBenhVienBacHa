using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public enum NhomChiPhiKhamChuaBenh
    {
        [Description("Dịch vụ khám bệnh")]
        DichVuKhamBenh = 1,
        [Description("Dịch vụ kỹ thuật")]
        DichVuKyThuat = 2,
        [Description("Dịch vụ giường")]
        DichVuGiuong = 3,
        [Description("Vật tư tiêu hao")]
        VatTuTieuHao = 4,
        [Description("Dược phẩm")]
        DuocPham = 5,
        [Description("Toa thuốc")]
        ToaThuoc = 6,
        [Description("Gói Dịch Vụ")]
        GoiDichVu = 7,
    }

    public class ChiPhiKhamChuaBenhVo : GridItem
    {
        public long? TaiKhoanBenhNhanChiId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public NhomChiPhiKhamChuaBenh LoaiNhom { get; set; }
        public NhomChiPhiBangKe NhomChiPhiBangKe { get; set; }
        public int SoThuTuGoiTrongNhomChiPhiBangKe { get; set; }
        public string Nhom { get; set; }
        public DateTime? NgayPhatSinh { get; set; }
        public string Khoa { get; set; }
        //public string Nhom => LoaiNhom.GetDescription();
        public int STT { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public long? LoaiGiaId { get; set; }
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

        //BVHD-3731
        public long? NoiDungGhiChuMiemGiamId { get; set; }

        public decimal DaThanhToan { get; set; }
        public decimal BNConPhaiThanhToan => ThanhTien - BHYTThanhToan - TongCongNo - SoTienMG - DaThanhToan;
        public string NoiThucHien { get; set; }
        public bool? DaThucHien { get; set; }
        //Biến này em dùng checkUi CheckedDefault = flalse 
        public bool CheckedDefault { get; set; }
        public bool CapNhatThanhToan { get; set; }

        public decimal TongCongNo => DanhSachCongNoChoThus?.Sum(o => o.SoTienCongNoChoThu) ?? SoTienBaoHiemTuNhanChiTra;
        public bool DaHoanThu { get; set; }
        public bool DuocHoanThu => DaThucHien != true && NgayThu != null && NgayThu.Value.Date != DateTime.Now.Date;
        public DateTime? NgayThu { get; set; }
        public IEnumerable<DanhSachCongNoVo> DanhSachCongNoChoThus { get; set; }
        public IEnumerable<DanhSachMienGiamVo> DanhSachMienGiamVos { get; set; }

        public string NguoiThu { get; set; }
        public string ThoiGianThuStr { get; set; }
        public int? SoPhieu { get; set; }
        public bool HuyDichVu { get; set; }
        public string DonViTinh { get; set; }

        //update 17/08/2020
        public DateTime ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhStr => ThoiDiemChiDinh.ApplyFormatDateTimeSACH();
        public decimal SoTienCongNo { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

    }

    public class DanhSachCongNoVo
    {
        public long CongTyBaoHiemTuNhanId { get; set; }
        public decimal SoTienCongNoChoThu { get; set; }
    }
    public class DanhSachMienGiamVo
    {
        public Enums.LoaiMienGiam LoaiMienGiam { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public long? DoiTuongUuDaiId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public string DoiTuongUuDai { get; set; }
        public long? TheVoucherId { get; set; }
        public string MaTheVoucher { get; set; }
        public Enums.LoaiChietKhau LoaiChietKhau { get; set; }
        public decimal SoTien { get; set; }
        public int? TiLe { get; set; }
    }

    public class DoiLoaiGiaDanhSachChiPhiKhamChuaBenh
    {
        public DoiLoaiGiaDanhSachChiPhiKhamChuaBenh()
        {
            ChiPhiKhamChuaBenhVos = new List<ChiPhiKhamChuaBenhVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? LoaiGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? LoaiGiaDichVuKyThuatBenhVienId { get; set; }
        public List<ChiPhiKhamChuaBenhVo> ChiPhiKhamChuaBenhVos { get; set; }
    }


    public class ChiPhiKhamChuaBenhTrongGoiDichVuVo : GridItem
    {
        public ChiPhiKhamChuaBenhTrongGoiDichVuVo()
        {
            DanhSachCongNoChoThus = new List<DanhSachCongNoVo>();
        }
        public int STT { get; set; }
        public long YeuCauGoiDichVuId { get; set; }
        public string TenChuongTrinhGoiDichVu { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public DateTime? NgayPhatSinh { get; set; }
        public NhomChiPhiKhamChuaBenh LoaiNhom { get; set; }
        public string Nhom => LoaiNhom.GetDescription();

        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)(Soluong * (double)DonGia);
        public bool KiemTraBHYTXacNhan { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public decimal BHYTThanhToan => DuocHuongBHYT ? (decimal)(Soluong * (double)DonGiaBHYTThanhToan) : 0;
        public decimal SoTienQuyetToan => ThanhTien - BHYTThanhToan - TongCongNo;
        //public decimal SoTienPhaiTraLaiBenhNhan => (decimal)(Soluong * (double)DonGiaBHYTThanhToan);
        public bool CheckedDefault { get; set; }
        public string NoiThucHien { get; set; }
        public decimal TongCongNo => DanhSachCongNoChoThus.Sum(o => o.SoTienCongNoChoThu);
        public List<DanhSachCongNoVo> DanhSachCongNoChoThus { get; set; }
    }


    public class ApDungKhuyenMaiBenhNhan : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public ApDungKhuyenMaiBenhNhan()
        {
            DsKhuyenMaiBenhNhans = new List<DanhSachDichVuKhuyenMaiBenhNhanVo>();
        }
        public List<DanhSachDichVuKhuyenMaiBenhNhanVo> DsKhuyenMaiBenhNhans { get; set; }
    }

    public class DanhSachDichVuKhuyenMaiBenhNhanVo : GridItem
    {
        public DanhSachDichVuKhuyenMaiBenhNhanVo()
        {
            DanhSachDichVuTrongGoiKhuyenMais = new List<DanhSachDichVuTrongGoiKhuyenMaiVo>();
        }
        public string TenGoi { get; set; }
        public List<DanhSachDichVuTrongGoiKhuyenMaiVo> DanhSachDichVuTrongGoiKhuyenMais { get; set; }
    }

    public class DanhSachDichVuTrongGoiKhuyenMaiVo
    {
        public long Id { get; set; }
        public bool DaChon { get; set; }
        public int STT { get; set; }
        public NhomChiPhiKhamChuaBenh LoaiNhom { get; set; }
        public string Nhom => LoaiNhom.GetDescription();
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaKM { get; set; }
        public string GhiChu { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDateTimeSACH();
        public decimal ThanhTien => (decimal)Soluong * DonGia;
        public decimal ThanhTienKM => (decimal)Soluong * DonGiaKM;
        public decimal SoTienMG => (ThanhTien - ThanhTienKM) > 0 ? (ThanhTien - ThanhTienKM) : 0;
        public double SoLuongDaDung { get; set; }
    }
}
