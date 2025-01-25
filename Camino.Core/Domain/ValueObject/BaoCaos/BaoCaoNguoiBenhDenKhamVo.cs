using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoNguoiBenhDenKhamQueryInfoVo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
        public string Hosting { get; set; }
    }

    public class BaoCaoNguoiBenhDenKhamGridVo : GridItem
    {
        public int STT { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime ThoiGianTiepNhan { get; set; }
        public string ThoiGianTiepNhanDisplay => ThoiGianTiepNhan.ApplyFormatTime();
        public DateTime? ThoiGianBatDauKham { get; set; }
        public string ThoiGianBatDauKhamDisplay => ThoiGianBatDauKham?.ApplyFormatTime();
        public DateTime? ThoiGianKetThucKham { get; set; }
        public string ThoiGianKetThucKhamDisplay => ThoiGianKetThucKham?.ApplyFormatTime();
        public string NgayKhamDisplay => ThoiGianBatDauKham?.ApplyFormatDate();
        public string TenNguoiBenh { get; set; }
        public string SoDienThoai { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public int? MucHuong { get; set; }
        public bool? CoBHYT { get; set; }
        public string DoiTuong
        {
            get
            {
                var doiTuong = string.Empty;
                if (LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                {
                    doiTuong = "Khám sức khỏe";
                }
                else
                {
                    if (MucHuong != null && MucHuong != 0 && CoBHYT == true)
                    {
                        doiTuong = $"BHYT ({MucHuong}%)";
                    }
                    else
                    {
                        doiTuong = "Viện phí";
                    }
                }
                return doiTuong;
            }
        }
        public string GioiTinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string DiaChi { get; set; }
        public string BacSiKham { get; set; }
        public string ChuanDoanIcd { get; set; }
        public string DichVuThucHien { get; set; }
        public string DichVuKhamThucHien { get; set; }
        public string DichVuKyThuatThucHien { get; set; }
        public string KetQua { get; set; }
        public string HuongGiaiQuyet { get; set; }
        public DateTime? NgayHenTaiKham { get; set; }
        public string NgayHenTaiKhamDisplay => NgayHenTaiKham?.ApplyFormatDate();
        public string NguoiGioiThieu { get; set; }
    }

    public class BaoCaoNguoiBenhDenKhamICDVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public string TenICD { get; set; }
        public string MaICD { get; set; }
        public string TenHienThi => $"{MaICD} - {TenICD}";
    }

    public class BaoCaoNguoiBenhDenKhamDichVuKyThuatVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public string TenDichVu { get; set; }
        public List<KetQuaPhienXetNghiemChiTietVo> KetQuaPhienXetNghiemChiTietVos { get; set; }
    }
}
