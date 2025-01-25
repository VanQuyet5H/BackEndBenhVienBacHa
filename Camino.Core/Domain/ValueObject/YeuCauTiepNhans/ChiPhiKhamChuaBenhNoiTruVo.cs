using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ChiPhiKhamChuaBenhNoiTruVo : GridItem
    {
        public long? TaiKhoanBenhNhanChiId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public string Khoa { get; set; }
        public DateTime? NgayPhatSinh { get; set; }
        public string NgayPhatSinhDiplay => NgayPhatSinh?.ToString("dd/MM/yyyy");

        public long DichVuBenhVienId { get; set; }
        public NhomChiPhiNoiTru LoaiNhom { get; set; }
        public NhomChiPhiBangKe NhomChiPhiBangKe { get; set; }
        public int SoThuTuGoiTrongNhomChiPhiBangKe { get; set; }
        public string Nhom { get; set; }

        public int STT { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public string DonViTinh { get; set; }
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal ThanhTien => KhongTinhPhi == true ? 0 : (decimal)(Soluong * (double)DonGia);
        public long LoaiGiaId { get; set; }
        public string MaSoTheBHYT { get; set; }
        public bool KiemTraBHYTXacNhan { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public decimal BHYTThanhToan => DuocHuongBHYT ? (decimal)(Soluong * (double)DonGiaBHYTThanhToan) : 0;

        public decimal SoTienMG { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public long? NoiDungGhiChuMiemGiamId { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal BNConPhaiThanhToan => ThanhTien - BHYTThanhToan - TongCongNo - SoTienMG - DaThanhToan;

        public bool CheckedDefault { get; set; }
        public bool CapNhatThanhToan { get; set; }
        public bool DichVuNgoaiTru { get; set; }
        public Enums.DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public decimal SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal TongCongNo => DanhSachCongNoChoThus?.Sum(o => o.SoTienCongNoChoThu) ?? SoTienBaoHiemTuNhanChiTra;
        public bool DaHoanThu { get; set; }
        public bool? DaThucHien { get; set; }
        public bool DuocHoanThu => DaThucHien != true && NgayThu != null && NgayThu.Value.Date != DateTime.Now.Date;
        public DateTime? NgayThu { get; set; }
        public List<long> YeuCauDichVuGiuongBenhVienChiPhiBHYTIds { get; set; }
        public IEnumerable<DanhSachCongNoVo> DanhSachCongNoChoThus { get; set; } = new List<DanhSachCongNoVo>();
        public IEnumerable<DanhSachMienGiamVo> DanhSachMienGiamVos { get; set; } = new List<DanhSachMienGiamVo>();
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
    }
    public enum NhomChiPhiNoiTru
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
        [Description("Truyền máu")]
        TruyenMau = 8,
    }
    public enum NhomChiPhiBangKe
    {
        [Description("Khám bệnh")]
        KhamBenh = 1,

        [Description("Ngày giường điều trị ban ngày")]
        NgayGiuongDieuTriBanNgay = 21,
        [Description("Ngày giường điều trị nội trú")]
        NgayGiuongDieuTriNoiTru = 22,

        [Description("Xét nghiệm")]
        XetNgiem = 3,
        [Description("Chẩn đoán hình ảnh")]
        ChuanDoanHinhAnh = 4,
        [Description("Thăm dò chức năng")]
        ThamDoChucNang = 5,
        [Description("Thủ thuật, phẫu thuật")]
        ThuThuatPhauThuat = 6,
        [Description("Máu, chế phẩm máu, vận chuyển máu")]
        Mau = 7,
        [Description("Thuốc, dịch truyền")]
        ThuocDichTruyen = 8,
        [Description("Vật tư y tế")]
        VatTuYte = 9,
        [Description("Gói vật tư")]
        GoiVatTu = 10,
        [Description("Vận chuyển người bệnh")]
        VanChuyenNguoiBenh = 11,
        [Description("Dịch vụ khác")]
        DichVuKhac = 12,
    }



    public class ApDungKhuyenMaiBenhNhanNoiTru : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public ApDungKhuyenMaiBenhNhanNoiTru()
        {
            DsKhuyenMaiBenhNhanNoiTrus = new List<DanhSachDichVuKhuyenMaiBenhNhanNoiTruVo>();
        }
        public List<DanhSachDichVuKhuyenMaiBenhNhanNoiTruVo> DsKhuyenMaiBenhNhanNoiTrus { get; set; }
    }

    public class DanhSachDichVuKhuyenMaiBenhNhanNoiTruVo : GridItem
    {
        public DanhSachDichVuKhuyenMaiBenhNhanNoiTruVo()
        {
            DanhSachDichVuTrongGoiKhuyenMaiNoiTrus = new List<DanhSachDichVuTrongGoiKhuyenMaiNoiTruVo>();
        }
        public string TenGoi { get; set; }
        public List<DanhSachDichVuTrongGoiKhuyenMaiNoiTruVo> DanhSachDichVuTrongGoiKhuyenMaiNoiTrus { get; set; }
    }

    public class DanhSachDichVuTrongGoiKhuyenMaiNoiTruVo
    {
        public long Id { get; set; }
        public bool DaChon { get; set; }
        public int STT { get; set; }
        public NhomChiPhiNoiTru LoaiNhom { get; set; }
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
