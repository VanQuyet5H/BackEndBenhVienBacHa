using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class ChiTietBaoHiemThanhToanVo
    {
        public ChiTietBaoHiemThanhToanVo()
        {
            DonGiaThems = new List<decimal>();
        }
        public long YeuCauKhamBenhId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public int LanKhamCoBHTrongNgay { get; set; }
        public decimal TiLeBaoHiemThanhToan { get; set; }
        public decimal MucHuong { get; set; }
        public List<decimal> DonGiaThems { get; set; }
        public decimal SoTienBHTTToanBo { get; set; }
        public bool IsUpdate { get; set; }
    }

    public class LookupItemNhomDichVuVo
    {
        public string DisplayName { get; set; }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
    }

    public class ThongTinDichVuKhamTiepTheo : ChiTietBaoHiemThanhToanVo
    {
        public long DichVuKhamBenhId { get; set; }
        public long LoaiGiaId { get; set; }
        public decimal GiaDichVuKham { get; set; }
        public decimal GiaBHTT { get; set; }
        public bool FlagDuocHuongBaoHiem { get; set; }
    }
    public class ListDichVuChiDinh
    {
        public long nhomChiDinhId { get; set; }
        public long dichVuChiDinhId { get; set; }
        public string TenNhom { get; set; }
        public int ThuTuIn { get; set; }
    }

    public class ListDichVuChiDinhTheoNguoiChiDinh
    {
        public long nhomChiDinhId { get; set; }
        public long dichVuChiDinhId { get; set; }
        public string TenNhom { get; set; }
        public int ThuTuIn { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
    }
    public class DataInChiDinh {
        public string LogoUrl { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinhString { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string DienThoai { get; set; }
        public string DoiTuong { get; set; }
        public string SoTheBHYT { get; set; }
        public string HanThe { get; set; }
        public string Now { get; set; }
        public string NowTime { get; set; }
        public string NoiYeuCau { get; set; }
        public string ChuanDoanSoBo { get; set; }
        public string DanhSachDichVu { get; set; }
        public string NguoiChiDinh { get; set; }
        public string NguoiGiamHo { get; set; }
        public string TenQuanHeThanNhan { get; set; }
        public string PhieuThu { get; set; }
        public string GhiChuCanLamSang { get; set; }
        public string NgayThangNam { get; set; }
    }
    public class DichVuChiDinhInGridVo
    {
        public long NhomChiDinhId { get; set; }
        public long DichVuChiDinhId { get; set; }
    }
    public class InChiDinhDichVuKhamNoiTruGridVo
    {
        public InChiDinhDichVuKhamNoiTruGridVo()
        {
            DichVuKhamNoiTruCanIns = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public string HosTingName { get; set; }
        public List<ListDichVuChiDinhTheoNguoiChiDinh> DichVuKhamNoiTruCanIns { get; set; }
        public bool KieuIn { get; set; }
        public long inChungChiDinh { get; set; }
    }
    public class ListDichVuChiDinhInffo
    {
        public EnumNhomGoiDichVu NhomChiDinhId { get; set; }
        public long DichVuChiDinhId { get; set; }
        public string TenNhom { get; set; }
        public int ThuTuIn { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public DateTime ThoiDiemDangKy { get; set; }
        public DateTime ThoiDiemChiDinh => new DateTime(ThoiDiemDangKy.Year,ThoiDiemDangKy.Month,ThoiDiemDangKy.Day, 0, 0, 0);
    }
    public class AddChiDinhTheoNguoiChiDinhVaNhomDichVuKhamNoiTru
    {
        public long YeuCauTiepNhanId { get; set; }
        public List<ListDichVuChiDinhInffo> ListDichVuTheoNguoiChiDinh { get; set; }
        public List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> ListDVK { get; set; }
        public string Content { get; set; }
        public string HostingName { get; set; }
    }
}
