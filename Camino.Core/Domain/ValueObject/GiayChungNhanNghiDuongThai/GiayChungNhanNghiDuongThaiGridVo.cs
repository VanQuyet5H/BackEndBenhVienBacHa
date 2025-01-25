using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.GiayChungNhanNghiDuongThai
{
    public class GiayChungNhanNghiDuongThaiGrid : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
    }
    public class ThongTinChungNhanNghiDuongThai
    {
        public string TenNhanVien { get; set; }
        public string ChanDoanVaPhuongPhapDieuTri { get; set; }
        public string NgayThucHienDisplay { get; set; }

    }
    public class PhuongPhapDieuTriModel
    {
        public string PhuongPhapDieuTri { get; set; }
    }
    public class XacNhanInPhieuGiayChungNhanNghiDuongThai
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
    }
    public class InPhieuGiayChungNhanNghiDuongThai
    {
        public string MauSo { get; set; }

        public string SoSeri { get; set; }
        public string ChanDoan { get; set; }
        public double? SoNgayNghi { get; set; }
        public DateTime NghiTuNgay { get; set; }
        public DateTime NghiDenNgay { get; set; }
        public string NguoiHanhNgheKBCB { get; set; }
        public string MaNguoiHanhNgheKBCB { get; set; }
        public string ThuTruongDonVi { get; set; }
        public string NghiTuNgayDisplay { get; set; }
        public string NghiDenNgayDisplay { get; set; }
    }
}
