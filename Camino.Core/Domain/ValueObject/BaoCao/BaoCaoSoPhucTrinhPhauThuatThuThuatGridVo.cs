using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoSoPhucTrinhPhauThuatThuThuatGridVo: GridItem
    {
        public int STT { get; set; }
        public long YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }
        public string MaTN { get; set; }
        public string TenBenhNhan { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string BHYT { get; set; }
        public string ChuanDoanTruocPt { get; set; }
        public string ChuanDoanSauPt { get; set; }
        public string PhuongPhapPhauThuat { get; set; }

        //BVHD-3636 
        public string ThuThuatChuyenKhoa { get; set; }
        public string SoThuTuThongTu50 { get; set; }


        public string PhuongPhapVoCam { get; set; }
        public string NhomDichVu { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public DateTime? ThoiGianBatDauPhauThuat { get; set; }
        public string ThoiGianPhauThuatStart => ThoiGianBatDauPhauThuat?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiGianKetThucPhauThuat { get; set; }
        public string ThoiGianPhauThuatEnd => ThoiGianKetThucPhauThuat?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiGianBatDauGayMe { get; set; }
        public string ThoiGianKhoiMe => ThoiGianBatDauGayMe?.ApplyFormatDateTimeSACH();
        public string LoaiPTTT { get; set; }
        public string NoiChiDinh { get; set; }
        public string NoiThucHien { get; set; }

        //ekip
        public string PTVChinh { get; set; }
        public string NguoiGayMeChinh { get; set; }
        public string NguoiGayMePhu { get; set; }
        public string PTVPhu { get; set; }
        public string DungCuVongTrong { get; set; }
        public string DungCuVongNgoai { get; set; }
        public string NguoiGayMeGayTePhu { get; set; }
        public string ChayNgoai { get; set; }
        public string Phu1 { get; set; }
        public string Phu2 { get; set; }
        public string Phu3 { get; set; }
    }

    public class BaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo
    {
        public long YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }
        public string HoTen { get; set; }
        public Enums.EnumVaiTroBacSi? VaiTroBacSi { get; set; }
        public Enums.EnumVaiTroDieuDuong? VaiTroDieuDuong { get; set; }
    }

    public class ExcelBaoCaoSoPhucTrinhPhauThuatThuThuatEkipVo
    {
        public string CellName { get; set; }
        public Enums.EnumVaiTroBacSi? VaiTroBacSi { get; set; }
        public Enums.EnumVaiTroDieuDuong? VaiTroDieuDuong { get; set; }
    }

    public class ItemNoiThucHienBaoCaoPTTTVo
    {
        public long KhoaId { get; set; }
        public long? PhongId { get; set; }
    }
}