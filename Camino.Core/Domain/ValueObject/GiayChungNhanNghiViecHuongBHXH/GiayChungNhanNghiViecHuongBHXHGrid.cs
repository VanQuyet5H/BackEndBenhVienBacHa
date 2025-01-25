using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH
{
    public class GiayChungNhanNghiViecHuongBHXHGrid :GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
    }
    public class ThongTinChungNhanNghiViecHuongBHXH
    {
        public string TenNhanVien { get; set; }
        public string ChanDoanVaPhuongPhapDieuTri { get; set; }
        public string NgayThucHienDisplay { get; set; }

    }
    public class PhuongPhapDieuTriModel
    {
        public string PhuongPhapDieuTri { get; set; }
    }
    public class XacNhanInPhieuGiayChungNhanNghiViecHuongBHXH
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
    }
    public class InPhieuGiayChungNhanNghiViecHuongBHXH
    {
        public string MauSo { get; set; }

        public string SoSeri { get; set; }
        public string ChanDoanVaPhuongPhapDieuTri { get; set; }
        public int SoNgayNghi { get; set; }
        public string HoTenCha { get; set; }
        public string HoTenMe { get; set; }
        public DateTime NghiTuNgay { get; set; }
        public DateTime NghiDenNgay { get; set; }
        public string NguoiHanhNgheKBCB { get; set; }
        public string MaHanhNgheKBCB { get; set; }
        public string ThuTruongDonVi { get; set; }
        //public string MaThuTruongDonVi { get; set; }
        public string NghiTuNgayDisplay { get; set; }
        public string NghiDenNgayDisplay { get; set; }
    }
    public class GiayChungNhanNghiViecHuongBHXHQueryInfo
    {
        public string Searching { get; set; }
    }
    public class GiayNghiHuongBHXHQueryInfo
    {
        public long? YeuCauTiepNhanNoiTruId { get; set; }
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }
        public long? NoiTruHoSoKhacId { get; set; }
    }
}
