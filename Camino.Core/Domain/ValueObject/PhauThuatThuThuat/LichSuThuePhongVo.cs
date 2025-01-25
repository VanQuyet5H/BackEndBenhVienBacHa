using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class LichSuThuePhongGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatThuePhongId { get; set; }
        public long? YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaNB { get; set; }
        public string TenNB { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string DiaChi { get; set; }
        public bool? CoBHYT { get; set; }
        public string DoiTuong => CoBHYT.GetValueOrDefault() != true ? "Viện phí" : "BHYT";
        public string DichVuThue { get; set; }
        public string LoaiPhongThue { get; set; }
        public DateTime? BatDauThue { get; set; }
        public string BatDauThueDisplay => BatDauThue?.ApplyFormatDateTime();
        public DateTime? KetThucThue { get; set; }
        public string KetThucThueDisplay => KetThucThue?.ApplyFormatDateTime();
        public string PhongThucHien { get; set; }
        public string BacSiGayMe { get; set; }
        public string PhauThuatVien { get; set; }
        public long CauHinhThuePhongId { get; set; }

    }

    public class LichSuThuePhongTimKiemVo
    {
        public string SearchString { get; set; }
        public long? CauHinhThuePhongId { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class ThongTinEkipVo
    {
        public long YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }
        public string HoTen { get; set; }
        public bool LaPhauThuatVien { get; set; }
    }

    public class LichSuThuePhongThongTinHanhChinhVo
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string MaNB { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? Tuoi { get; set; }
        public string SoDienThoai { get; set; }
        public string DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string TuyenKham { get; set; }
        public string SoBHYT { get; set; }
        public int? MucHuong { get; set; }
        public string NgayHieuLuc => BHYTNgayHieuLuc?.ApplyFormatDate();
        public string LyDoTiepNhan { get; set; }
        public string NgayTiepNhan { get; set; }

        public bool CoBHYT { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public bool IsBHYTHetHan => CoBHYT && BHYTNgayHetHan != null && BHYTNgayHetHan.Value.Date < DateTime.Now.Date;

        public bool LaCapCuu { get; set; }
        public bool QuyetToanTheoNoiTru { get; set; }

        //BVHD-3941
        public long? YeuCauTiepNhanId { get; set; }
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class LookupYeuCauCoThuePhongVo : LookupItemVo
    {
        public long? ThuePhongId { get; set; }
        public long? CauHinhThuePhongId { get; set; }
        public string TenCauHinhThuePhong { get; set; }
        public long YeuCauDichVuKyThuatThuePhongId { get; set; }
        public DateTime? ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemKetThuc { get; set; }
        public bool CoThuePhong => ThuePhongId != null;
        public long? NoiThucHienId { get; set; }
        public string TenNoiThucHien { get; set; }
    }
}
