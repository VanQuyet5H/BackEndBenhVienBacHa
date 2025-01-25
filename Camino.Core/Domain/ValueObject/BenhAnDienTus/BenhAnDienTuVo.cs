using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.BenhAnDienTus
{
    public class BenhAnDienTuTimKiemVo
    {
        public string SearchString { get; set; }
        public BenhAnDienTuTimKiemTuNgayDenNgayVo TuNgayDenNgayNhapVien { get; set; }
        public BenhAnDienTuTimKiemTuNgayDenNgayVo TuNgayDenNgayXuatVien { get; set; }
    }

    public class BenhAnDienTuTimKiemTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class BenhAnDienTuNoiTruBenhAnGridVo : GridItem
    {
        public string SoBenhAn { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public string SoChungMinhThu { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string NgaySinhFormat { get; set; }
    }

    public class BenhAnDienTuDetailVo
    {
        public BenhAnDienTuDetailVo()
        {
            GayBenhAns = new List<BenhAnDienTuMenuInfoVo>();
        }
        public long NoiTruBenhAnId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public long BenhNhanId { get; set; }
        public string Hosting { get; set; }
        public bool LaInPhieu { get; set; }

        public List<BenhAnDienTuMenuInfoVo> GayBenhAns { get; set; }
    }

    public class BenhAnDienTuMenuInfoVo
    {
        public BenhAnDienTuMenuInfoVo()
        {
            ChiTietHoSos = new List<BenhAnDienTuChiTietMenuInfo>();
        }
        public int ViTri { get; set; }
        public string TenGayHoSo { get; set; }
        public List<BenhAnDienTuChiTietMenuInfo> ChiTietHoSos { get; set; }
    }

    public class BenhAnDienTuChiTietMenuInfo
    {
        public Enums.LoaiPhieuHoSoBenhAnDienTu LoaiHoSo { get; set; }
        public long? Value { get; set; }
    }

    public class BenhAnDienTuThongTinDichVuVo
    {
        public long Id { get; set; }
        public long DichVuBenhVienId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public bool CoNhapVien { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public bool DaThucHien { get; set; }
        public long? NhomBenhVienId { get; set; }
    }

    #region Phiếu in bìa bệnh án
    public class BiaBenhAnDienTuInVo
    {
        public long NoiTruBenhAnId { get; set; }
        public string Hosting { get; set; }
    }

    public class ThongTinBiaBenhAnVo
    {
        public string Header { get; set; }
        public string LogoUrl { get; set; }
        public string SoLuuTru { get; set; }
        public string MaVaoVien { get; set; }
        public string HoVaTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string BenhNhanTuoi => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string LaNam => GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? "X" : "";
        public string LaNu => GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu ? "X" : "";
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string ChanDoan { get; set; }
        public string MaChanDoan { get; set; }
        public bool LaBenhAnSanKhoa { get; set; }

        public bool CoBHYT { get; set; }

        public string KhoaNhapVien { get; set; }
        public long KhoaNhapVienId { get; set; }
        public DateTime? NgayNhapVienDayDu { get; set; }
        public string NgayNhapVien => NgayNhapVienDayDu?.Day.ToString();
        public string ThangNhapVien => NgayNhapVienDayDu?.Month.ToString();
        public string NamNhapVien => NgayNhapVienDayDu?.Year.ToString().Substring(2, 2);

        public string ThongTinKhoaChuyen { get; set; }
        //public string KhoaChuyen { get; set; }
        //public DateTime? NgayChuyenKhoaDayDu { get; set; }
        //public string NgayChuyenKhoa => NgayChuyenKhoaDayDu?.Day.ToString();
        //public string ThangChuyenKhoa => NgayChuyenKhoaDayDu?.Month.ToString();
        //public string NamChuyenKhoa => NgayChuyenKhoaDayDu?.Year.ToString().Substring(2, 2);

        public string KhoaXuatVien { get; set; }
        public DateTime? NgayXuatVienDayDu { get; set; }
        public string NgayXuatVien => NgayXuatVienDayDu?.Day.ToString();
        public string ThangXuatVien => NgayXuatVienDayDu?.Month.ToString();
        public string NamXuatVien => NgayXuatVienDayDu?.Year.ToString().Substring(2, 2);

        public int NamHienTai => DateTime.Now.Year;

        // cập -Năm BA: theo năm NB ra viện(hiện tại Đang mặc định năm 2021)
        public string NamNBRaVien { get; set; }
        public Enums.EnumHinhThucRaVien? HinhThucRaVien { get; set; }
    }

    public class ThongTinChuyenKhoaVo
    {
        public string KhoaChuyen { get; set; }
        public DateTime? NgayChuyenKhoaDayDu { get; set; }
        public string NgayChuyenKhoa => NgayChuyenKhoaDayDu?.Day.ToString();
        public string ThangChuyenKhoa => NgayChuyenKhoaDayDu?.Month.ToString();
        public string NamChuyenKhoa => NgayChuyenKhoaDayDu?.Year.ToString().Substring(2, 2);
    }
    #endregion

    public class GayBenhAnVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int ViTri { get; set; }
        public string TenPhieuHoSo { get; set; }
        public bool? IsDisabled { get; set; }
    }
    public class GayBenhAnChiTietVo : GridItem
    {
        public long GayBenhAnId { get; set; }
        public LoaiPhieuHoSoBenhAnDienTu LoaiPhieuHoSoBenhAnDienTu { get; set; }
        public long? Value { get; set; }
        public string TenValue { get; set; }
    }

    public class GayBenhAnExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(20)]
        public string Ten { get; set; }
        [Width(20)]
        public string ViTri { get; set; }
        [Width(50)]
        public string TenPhieuHoSo { get; set; }
        public bool? IsDisabled { get; set; }
        [Width(20)]
        public string TenTrangThai => IsDisabled != true ? "Đang sử dụng" : "Ngừng sử dụng";
    }

    public class PhieuHoSoGayBenhAnLookupVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
    }

    public class KeyIdStringPhieuHoSoGayBenhAnLookupVo
    {
        public long PhieuHoSoId { get; set; }
        public PhieuHoSoBenhAn LoaiPhieuHoSoBenhAn { get; set; }
    }
}
