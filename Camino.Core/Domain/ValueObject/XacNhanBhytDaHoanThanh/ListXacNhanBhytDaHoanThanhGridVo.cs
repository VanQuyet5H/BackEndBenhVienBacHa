using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh
{
    public class ListXacNhanBhytDaHoanThanhGridVo : GridItem
    {
        public string MaTn { get; set; }

        public string MaBn { get; set; }

        public string HoTen { get; set; }

        public string NamSinh { get; set; }

        public string GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        public string SoDienThoaiDisplay { get; set; }

        public decimal? SoTienDaXacNhan =>
                (DanhSachDichVuDuocHuongBhyt?.DichVuKhamBenhDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
                (DanhSachDichVuDuocHuongBhyt?.DichVuKyThuatDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
                (DanhSachDichVuDuocHuongBhyt?.DichVuGiuongDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
                (DanhSachDichVuDuocHuongBhyt?.DuocPhamDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
                (DanhSachDichVuDuocHuongBhyt?.ToaThuocDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
                (DanhSachDichVuDuocHuongBhyt?.VatTuBenhVienDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0);

        public ListDichVuDuocHuongBhytHoanThanhVo DanhSachDichVuDuocHuongBhyt { get; set; }

        public DateTime? ThoiDiemDuyetBaoHiem => ThoiDiemDuyetBaoHiems?.OrderBy(o => o).LastOrDefault();
        public string ThoiDiemDuyetBaoHiemDisplayName => ThoiDiemDuyetBaoHiem?.ApplyFormatDate();
        public List<DateTime> ThoiDiemDuyetBaoHiems { get; set; }
    }

    public class ListDichVuDuocHuongBhytHoanThanhVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public IEnumerable<DichVuDuocHuongBhytHoanThanhVo> DichVuKhamBenhDuocHuongBhyt { get; set; }
        public IEnumerable<DichVuDuocHuongBhytHoanThanhVo> DichVuKyThuatDuocHuongBhyt { get; set; }
        public IEnumerable<DichVuDuocHuongBhytHoanThanhVo> DichVuGiuongDuocHuongBhyt { get; set; }
        public IEnumerable<DichVuDuocHuongBhytHoanThanhVo> DuocPhamDuocHuongBhyt { get; set; }
        public IEnumerable<DichVuDuocHuongBhytHoanThanhVo> ToaThuocDuocHuongBhyt { get; set; }

        public IEnumerable<DichVuDuocHuongBhytHoanThanhVo> VatTuBenhVienDuocHuongBhyt { get; set; }
    }
    public class DichVuDuocHuongBhytHoanThanhVo
    {
        public double Soluong { get; set; }
        public bool? BaoHiemChiTra { get; set; }

        public decimal SoTienDaXacNhan => BaoHiemChiTra == true ? (decimal)(Soluong * (double)DonGiaBhytThanhToan.GetValueOrDefault()) : 0;

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }

        public decimal? DgThamKhao { get; set; }

        public double PhanTramCuThe => (double)(TiLeDv * MucHuong) / 100;

        public decimal? DonGiaBhytThanhToan => (decimal)PhanTramCuThe * DgThamKhao.GetValueOrDefault() / 100;
    }
    public class DanhSachBHYTJson
    {
        public string ThoiDiemDuyetBaoHiemTu { get; set; }
        public string ThoiDiemDuyetBaoHiemDen { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
