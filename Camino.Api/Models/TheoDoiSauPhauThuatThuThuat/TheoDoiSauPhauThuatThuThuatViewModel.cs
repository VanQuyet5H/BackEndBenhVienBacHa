using iText.Layout.Element;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class TheoDoiSauPhauThuatThuThuatViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public EnumTrangThaiTheoDoiSauPhauThuatThuThuat TrangThai { get; set; }
        public DateTime? ThoiDiemBatDauTheoDoi { get; set; }
        public DateTime? ThoiDiemKetThucTheoDoi { get; set; }
        public long? BacSiPhuTrachTheoDoiId { get; set; }
        public string BacSiPhuTrachTheoDoiDisplay { get; set; }
        public long? DieuDuongPhuTrachTheoDoiId { get; set; }
        public string DieuDuongPhuTrachTheoDoiDisplay { get; set; }
        public string GhiChuTheoDoi { get; set; }
        public EnumTrangThaiPhauThuatThuThuat TrangThaiPhauThuatThuThuat { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public long PhongBenhVienId { get; set; }
        public bool IsChuyenGiaoTuTuongTrinh { get; set; }
    }
}