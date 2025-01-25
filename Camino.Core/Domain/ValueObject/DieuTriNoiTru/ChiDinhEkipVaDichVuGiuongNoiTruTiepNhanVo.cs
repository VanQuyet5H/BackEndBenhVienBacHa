using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long BacSiDieuTriId { get; set; }
        public long DieuDuongId { get; set; }
        public DateTime TuNgay { get; set; }
        public long DichVuGiuongId { get; set; }
        public long GiuongId { get; set; }
        public string TenGiuong { get; set; }
        public long LoaiGiuong { get; set; }
        public bool BaoPhong { get; set; }
        public DateTime ThoiGianNhan { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
    }

    public class GiuongBenhTrongVo
    {
        public GiuongBenhTrongVo()
        {
            ThoiGianNhan = DateTime.Now;
        }

        public long GiuongBenhId { get; set; }
        public bool? BaoPhong { get; set; }
        public DateTime ThoiGianNhan { get; set; }
        public DateTime? ThoiGianTra { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? YeuCauTiepNhanNoiTruId { get; set; }
    }

    public class DichVuGiuongTrongGoiVo
    {
        public long BenhNhanId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public long NhomGiaDichVuId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string MaDichVu { get; set; }
        public decimal? Gia { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
    }
}
