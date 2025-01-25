using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoBenhNhanKhamNgoaiTruVo
    {
        public long KhoaId { get; set; }
        public long PhongId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string Hosting { get; set; }
    }
    public class BaoCaoBenhNhanKhamNgoaiTruGridVo : GridItem
    {
        public DateTime? ThoiGianTiepNhan { get; set; }
        public string ThoiGianTiepNhanString => ThoiGianTiepNhan != null ? ThoiGianTiepNhan.GetValueOrDefault().ApplyFormatDateTimeSACH() : "";
        public string MaTN { get; set; }
        public string HoTenBn { get; set; }
        public string NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string TheBHYT { get; set; }
        public List<YeuCauKhamBenhVo> ListPhieuKham { get; set; }
        public string PhieuKham { get; set; }
        public string PhongKham { get; set; }
        public string NoiThucHien { get; set; }
        public long? PhongKhamId { get; set; }
        public string ICD { get; set; }
        public string TrangThai { get; set; }
        public string BsKham { get; set; }
        public string BsKetLuan { get; set; }
        public string ThoiGianThanhToan { get; set; }
        public string CachGiaiQuyet { get; set; }
        public string NgoaiGioHanhChinh { get; set; }
        //public string ThoiGianThanhToanString => ThoiGianThanhToan != null ? ThoiGianThanhToan.ApplyFormatDateTimeSACH() : "";
    }
    public class YeuCauKhamBenhVo
    {
        public string PhieuKham { get; set; }
        public string PhongKham { get; set; }
        public string ICD { get; set; }
        public string TrangThai { get; set; }
        public string BsKham { get; set; }
        public string BsKetLuan { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }
        public string CachGiaiQuyet { get; set; }
        public string NgoaiGioHanhChinh { get; set; }
        public string ThoiGianThanhToanString => ThoiGianThanhToan != null ? ThoiGianThanhToan.GetValueOrDefault().ApplyFormatDateTimeSACH() : "";
    }

    public class BaoCaoBenhNhanKhamNgoaiTruDemoGridVo : GridItem
    {
        public string ThoiGianBatDauTrongNgay => "07:00";
        public string ThoiGianKetThucTrongNgay => "16:45";
        public DateTime? ThoiGianKham { get; set; }
        public string ThoiGianKhamDisplay => ThoiGianKham?.ApplyFormatDateTimeSACH();
        public string CongTyKhamSucKhoe { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public Enums.LoaiGioiTinh? LoaiGioiTinh { get; set; }
        public string GioiTinh => LoaiGioiTinh?.GetDescription();
        public string TheBHYT { get; set; }
        public string PhieuKham { get; set; }
        public string PhongKham { get; set; }
        public string ICD { get; set; }
        public string TrangThai { get; set; }
        public string BacSiKham { get; set; }
        public string BacSiKetLuan { get; set; }
        public string CachGiaiQuyet { get; set; }
        public string KhoaNhapVien { get; set; }
        public bool SuDungGoi { get; set; }
        public bool NgoaiGioHanhChinh => ThoiGianKham != null
                                         && (ThoiGianKham.Value.TimeOfDay < System.TimeSpan.Parse(ThoiGianBatDauTrongNgay)
                                            || ThoiGianKham.Value.TimeOfDay > System.TimeSpan.Parse(ThoiGianKetThucTrongNgay));

        public string NoiThucHien { get; set; }
        public long? PhongKhamId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
    }
}
