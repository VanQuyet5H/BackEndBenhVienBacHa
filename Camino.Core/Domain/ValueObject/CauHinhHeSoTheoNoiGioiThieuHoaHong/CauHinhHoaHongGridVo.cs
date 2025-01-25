using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong
{
    public class CauHinhHoaHongGridVo
    {
    }
    public class CauHinhChiTietHoaHong : GridItem
    {
        public CauHinhChiTietHoaHong()
        {
            ThongTinCauHinhHoaHongs = new List<ThongTinCauHinhHoaHongGridVo>();
        }
        public string Ten { get; set; }
        public string TenNoiGioiThieuHopDong { get; set; }
        public long NoiGioiThieuId { get; set; }
        public string Donvi { get; set; }
        public string SoDienThoai { get; set; }
        public string MoTa { get; set; }
        public long NoiGioiThieuHopDongId { get; set; }
        public long ChonLoaiDichVuId { get; set; }

        public List<ThongTinCauHinhHoaHongGridVo> ThongTinCauHinhHoaHongs { get; set; }
    }
    public class ThongTinCauHinhHoaHongGridVo : GridItem
    {

        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }

        public long? TenNhomGiaId { get; set; }
        public string TenNhomGia { get; set; }

        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }

        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }

        public string GhiChu { get; set; }
        public string TenDichVu { get; set; }
        public string MaDichVu { get; set; }

        public string Nhom { get; set; }
        public decimal? DonGia { get; set; }

        public bool LaDichVuKham { get; set; }
        public bool LaDichVuKyThuat { get; set; }
        public bool LaDichVuGiuong { get; set; }
        public bool LaDuocPham { get; set; }
        public bool LaVatTu { get; set; }
        public bool? IsEdit { get; set; }



        public LoaiHoaHong ChonTienHayHoaHong { get; set; }
        public decimal? DonGiaHoaHongHoacTien { get; set; }
        public int? ADDHHTuLan { get; set; }
        public int? ADDHHDenLan { get; set; }
    }
}
