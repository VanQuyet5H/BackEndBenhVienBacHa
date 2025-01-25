using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.NoiGioiThieu
{
    public class ThongTinGiaVo
    {
        public long? NhomGiaId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public decimal? DonGia { get; set; }
    }

    public class NoiGioiThieuChiTietMienGiamGridVo : GridItem
    {
        public long? NoiGioiThieuId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public string TenNhomGia { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public Enums.LoaiChietKhau? LoaiChietKhau { get; set; }
        public int? TiLeChietKhau { get; set; }
        public decimal? SoTienChietKhau { get; set; }
        public string GhiChu { get; set; }

        public string Nhom
        {
            get
            {
                if (DichVuKhamBenhBenhVienId != null)
                {
                    return Enums.EnumNhomDichVu.KhamBenh.GetDescription();
                }
                else if (DichVuKyThuatBenhVienId != null)
                {
                    return Enums.EnumNhomDichVu.KyThuat.GetDescription();
                }
                else if (DichVuGiuongBenhVienId != null)
                {
                    return Enums.EnumNhomDichVu.GiuongBenh.GetDescription();
                }
                else if (DuocPhamBenhVienId != null)
                {
                    return Enums.EnumNhomDichVu.DuocPham.GetDescription();
                }
                else if (VatTuBenhVienId != null)
                {
                    return Enums.EnumNhomDichVu.VatTu.GetDescription();
                }
                return "Khác";
            }
        }

        public bool LaDichVuKham => DichVuKhamBenhBenhVienId != null;
        public bool LaDichVuKyThuat => DichVuKyThuatBenhVienId != null;
        public bool LaDichVuGiuong => DichVuGiuongBenhVienId != null;
        public bool LaDuocPham => DuocPhamBenhVienId != null;
        public bool LaVatTu => VatTuBenhVienId != null;

        public decimal? DonGia { get; set; }

        public decimal? DonGiaSauChietKhau
        {
            get
            {
                if (DonGia == null || DonGia == 0)
                {
                    return null;
                }

                decimal soTienGiam = 0;
                if (SoTienChietKhau != null)
                {
                    soTienGiam = SoTienChietKhau.Value;
                }
                else if (TiLeChietKhau != null)
                {
                    soTienGiam = (DonGia.GetValueOrDefault() / 100) * TiLeChietKhau.Value;
                }

                return soTienGiam > DonGia.GetValueOrDefault() ? DonGia : DonGia.GetValueOrDefault() - soTienGiam;
            }
        }
    }
}
