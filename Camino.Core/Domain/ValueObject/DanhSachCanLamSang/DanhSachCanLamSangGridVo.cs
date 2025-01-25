using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan
{
    public class DanhSachCanLamSangGridVo : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string HoTenRemoveDiacritics => HoTen.RemoveDiacritics();
        public int? NamSinh { get; set; }
        public string GioiTinhStr { get; set; }
        public string DiaChi { get; set; }
        public string DiaChiRemoveDiacritics => DiaChi.RemoveDiacritics();
        public string DienThoaiStr { get; set; }
        public DateTime? Ngay { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }
        public string NgayCoKetQuaStr { get; set; }
        public string NgayCoKetQuaSACHStr { get; set; }
        public DateTime NgayCoKetQua { get; set; }

        public string TuNgay { get; set; }
        public string DenNgay { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool? TrangThai { get; set; }
        public string TimKiem { get; set; }

        public bool ChuaCoKetQua { get; set; }
        public bool DaCoKetQua { get; set; }

        public string SearchString { get; set; }
    }

    public class TimKiemThongTinBenhNhan
    {
        public string TimKiemMaBNVaMaTN { get; set; }
    }
    public class KetQuaCLS
    {
        public Boolean Type { get; set; }
        public long Id { get; set; }
    }

    public class YeuCauKyThuatCDHALookupItemVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public bool TrangThaiDangThucHien { get; set; }
    }

    public class TrangThaiYeuCauKyThuat
    {
        public bool TrangThaiYeuCauTiepNhan { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string TenNguoiSuaSauCung { get; set; }
        public string ThoiGianSuaSauCung { get; set; }

        public bool TrangThaiDaThucHien { get; set; }
        public bool NguoiThucHien { get; set; }
    }
}