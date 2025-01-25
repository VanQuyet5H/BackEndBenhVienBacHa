using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class KhamBenhDangKhamGridVo : GridItem
    {
        public string Phong { get; set; }
        public long PhongBenhVienId { get; set; }
        public string TenPhongBenhVien { get; set; }
        public string Khoa { get; set; }
        public long KhoaId { get; set; }
        public string BacSiDangKham { get; set; }
        public string BenhNhanDangKham { get; set; }
        public int SoLuongBenhNhan { get; set; }
    }

    public class KhamBenhDangKhamTheoPhongKhamGridVo : GridItem
    {
        public long PhongBenhVienHangDoiId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long PhongBenhVienId { get; set; }
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiFilter { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay
        {
            get { return ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(); }
        }

        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThai { get; set; }
        public string TenTrangThai
        {
            get { return TrangThai.GetDescription(); }
        }
        public bool CoBaoHiem { get; set; }
        public int SoThuTu { get; set; }
    }
}
