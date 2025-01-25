using System;
using System.ComponentModel;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KhamDoanYeuCauNhanSuKhamSucKhoeGridVo : GridItem
    {
        public string HopDong { get; set; }

        public string TenCongTy { get; set; }

        public int SoLuongBn { get; set; }

        public int SoLuongBs { get; set; }

        public int SoLuongDd { get; set; }

        public int NhanVienKhac { get; set; }

        public DateTime NgayBatDauKham { get; set; }

        public string NgayBatDauKhamDisplay => NgayBatDauKham.ApplyFormatDate();

        public DateTime? NgayKetThucKham { get; set; }

        public string NgayKetThucKhamDisplay => NgayKetThucKham?.ApplyFormatDate();

        public DateTime NgayGui { get; set; }

        public string NgayGuiDisplay => NgayGui.ApplyFormatDateTime();

        public string NguoiGui { get; set; }

        public string KhthDuyet { get; set; }

        public DateTime? NgayKhthDuyet { get; set; }

        public string NgayKhthDuyetDisplay => NgayKhthDuyet != null
            ? NgayKhthDuyet.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public string NsDuyet { get; set; }

        public DateTime? NgayNsDuyet { get; set; }

        public string NgayNsDuyetDisplay => NgayNsDuyet != null
            ? NgayNsDuyet.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public string GdDuyet { get; set; }

        public DateTime? NgayGdDuyet { get; set; }

        public string NgayGdDuyetDisplay => NgayGdDuyet != null
            ? NgayGdDuyet.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public EnumTrangThaiKhamDoan TrangThai { get; set; }

        public string TenTinhTrang => TrangThai.GetDescription();
    }

    public enum EnumTrangThaiKhamDoan
    {
        [Description("Chờ duyệt")]
        ChoDuyet = 1,

        [Description("Đã duyệt")]
        DaDuyet = 2,

        [Description("Từ chối")]
        TuChoi = 3
    }
}
