using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class DataInPhieuDieuTriVaSerivcesVo
    {
        public string MaSoTiepNhan { get; set; }

        public string Khoa { get; set; }

        public string MaBn { get; set; }

        public string NhomMau { get; set; }

        public string HoTenNgBenh { get; set; }

        public int? NamSinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }

        public int TuoiNgBenh => NamSinh != null ? DateTime.Now.Year - NamSinh.GetValueOrDefault() : 0;

        public string Cmnd { get; set; }

        public string GTNgBenh { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string Buong { get; set; }

        public string Giuong { get; set; }

        public string ChanDoan { get; set; }

        public string DuKienPPDieuTri { get; set; }

        public string TienLuong { get; set; }

        public string NhungDieuCanLuuY { get; set; }

        public string BNSuDungBHYT { get; set; }

        public string ThongTinVeGiaDV { get; set; }

        public int Ngay { get; set; }

        public int Thang { get; set; }

        public int Nam { get; set; }

        public string HoTenBacSi { get; set; }

        public string ChanDoanRaVien { get; set; }

        public string ChanDoanVaoVien { get; set; }

        public DateTime? NgayVaoVien { get; set; }

        public DateTime? NgayRaVien { get; set; }
        public LoaiBenhAn LoaiBenhAn { get; set; }
}
    public class PhieuBienBanHoiChanPhauThuatGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuBienBanHoiChanPhauThuatGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuBienBanHoiChanPhauThuatGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }

    public class DanhSachBienBanHoiChanPhauThuat
    {
        public string NgayHoiChanDisplay { get; set; }
        public string ThanhVienThamGiaDisplay { get; set; }
        public string ChanDoan { get; set; }
        public long IdNoiTruHoSoKhac { get; set; }
    }
    public class DanhSachBienBanHoiChanPhauThuatJson
    {
        public string ThoiGianHoiChanUTC { get; set; }
        public List<ThanhVienThamGia> ThanhVienThamGias { get; set; }
        public string ChanDoan { get; set; }
    }

    public class PhieuInThongTinDieuTriVaCacDichVu
    {
        public string KhoaCreate { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string HoTenNgBenh { get; set; }
        public string NgayThangNamSinh { get; set; }
        public string GTNgBenh { get; set; }
        public string DiaChi { get; set; }
        public string Khoa { get; set; }
        public string Buong { get; set; }
        public string Giuong { get; set; }
        public string ChanDoan { get; set; }
        public string DuKienPPDieuTri { get; set; }
        public string TienLuong { get; set; }
        public string NhungDieuCanLuuY { get; set; }
        public string ThongTinVeGiaDV { get; set; }
        public string YKien { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string HoTenBacSi { get; set; }
        public string NguoiBenh { get; set; }
    }


}
