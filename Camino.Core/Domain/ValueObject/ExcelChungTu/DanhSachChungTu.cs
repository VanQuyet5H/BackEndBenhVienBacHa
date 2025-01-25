using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.ExcelChungTu
{
    public class TimKiemThongTinBenhNhanXuatExcelChungTuQueryInfo : QueryInfo
    {
        public LoaiChungTuXuatExcel? LoaiChungTu { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class DanhSachChungTuXuatExcelData : GridItem
    {
        public long? YeuCauTiepNhanNoiTruId { get; set; }
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }

        public long? YeuCauTiepNhanNgoaiTruCanQuyetToanId { get; set; }
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public bool? QuyetToanTheoNoiTru { get; set; }

        public DateTime ThoiGianTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangNam => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public DateTime? ThoiGianNhapVien { get; set; }

        public string GiayRaVien { get; set; }
        public string GiayChungNhanNghiViecHuongBHXH { get; set; }
        public string GiayNghiDuongThai { get; set; }
        public string TomTatHoSoBenhAn { get; set; }
        public string GiayChungSinh { get; set; }

        public string ChanDoanGhiChu { get; set; }
        public string PhuongPhapDieuTri { get; set; }

        public List<YeuCauKhamBenhThongTinDuongThai> YeuCauKhamBenhThongTinDuongThais { get; set; }
        public List<YeuCauKhamBenhThongTinNghiHuongBHXH> YeuCauKhamBenhThongTinNghiHuongBHXHs { get; set; }
        public List<long> YeuCauKhamBenhIdCoDuongThais { get; set; }
        public DateTime? ThoiGianRaVien { get; set; }

        public string ThongTinTongKetBenhAn { get; set; }
        public LoaiBenhAn LoaiBenhAn { get; set; }
        public List<ThongTinNoiTruPhieuDieuTri> ThongTinNoiTruPhieuDieuTris { get; set; }


        public DateTime? NgayTaoChungTu { get; set; }    
        //public NoiTruPhieuDieuTri NoiTruPhieuDieuTris { get; set; }

    }
    public class ThongTinNoiTruPhieuDieuTri
    {
        public DateTime NgayDieuTri { get; set; }
        public string ChanDoanChinhGhiChu { get; set; }
    }   
    public class YeuCauKhamBenhThongTinDuongThai
    {
        public DateTime? DuongThaiTuNgay { get; set; }
        public DateTime? DuongThaiDenNgay { get; set; }
        public string GhiChuICDChinh { get; set; }
        public DateTime? DuongThaiNgayIn { get; set; }
    }
    public class YeuCauKhamBenhThongTinNghiHuongBHXH
    {
        public DateTime? NghiTuNgay { get; set; }
        public DateTime? NghiDenNgay { get; set; }
        public string GhiChuICDChinh { get; set; }
        public string PhuongPhapDieuTri { get; set; }
        public DateTime? NghiHuongBHXHNgayIn { get; set; }
    }

    public class DanhSachChungTuXuatExcel : GridItem
    {
        public long? YeuCauTiepNhanNoiTruId { get; set; }
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }

        public string MaNB { get; set; }
        public string HoTen { get; set; }
        public string NgayThangNam { get; set; }
        public DateTime? ThoiGianTiepNhan { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();

        public string ChanDoan { get; set; }
        public string PhuongPhapDieuTri { get; set; }

        public DateTime? TuNgay { get; set; }
        public string TuNgayDisplay => TuNgay?.ApplyFormatDateTimeSACH();

        public DateTime? DenNgay { get; set; }
        public string DenNgayDisplay => DenNgay?.ApplyFormatDateTimeSACH();

        public DateTime? NgayTaoChungTu { get; set; }
        public string NgayTaoChungTuDisplay => NgayTaoChungTu?.ToString("dd/MM/yyyy");
    }

    public enum LoaiChungTuXuatExcel
    {
        [Description("Giấy ra viện")]
        GiayRaVien = 1,
        [Description("Giấy nghỉ hưởng BHXH")]
        GiayNghiHuongBHXH = 2,
        [Description("Giấy nghỉ dưỡng thai")]
        GiayNghiDuongThai = 3,
        [Description("Tóm tắt bệnh án")]
        TomTatBenhAn = 4,
        [Description("Giấy chứng sinh")]
        GiayChungSinh = 5
    }
}
