using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.NoiGioiThieu
{
    public class NoiGioiThieuChiTietMienGiamViewModel : BaseViewModel
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

        public bool LaDichVuKham { get; set; }
        public bool LaDichVuKyThuat { get; set; }
        public bool LaDichVuGiuong { get; set; }
        public bool LaDuocPham { get; set; }
        public bool LaVatTu { get; set; }

        public decimal? DonGia { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
    }
}
