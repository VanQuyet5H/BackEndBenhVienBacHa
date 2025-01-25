using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.CauhinhHeSoTheoNoiGioiThieuHoaHong
{
    public class CauHinhHeSoTheoThoiGianHoaHongViewModel : BaseViewModel
    {
        public CauHinhHeSoTheoThoiGianHoaHongViewModel()
        {
            ThongTinCauHinhHeSoTheoNoiGtHoaHongs = new List<ThongTinCauHinhHeSoTheoNoiGtHoaHong>();
        }
        public string Ten { get; set; }
        public long NoiGioiThieuId { get; set; }
        public string Donvi { get; set; }
        public string SoDienThoai { get; set; }
        public string MoTa { get; set; }
        public long NoiGioiThieuHopDongId { get; set; }
        public long ChonLoaiDichVuId { get; set; }
        public List<ThongTinCauHinhHeSoTheoNoiGtHoaHong> ThongTinCauHinhHeSoTheoNoiGtHoaHongs { get; set; }
    }
    public class ThongTinCauHinhHeSoTheoNoiGtHoaHong : BaseViewModel
    {

        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }

        //public long TenNhomGiaId  { get; set; }
        public string TenNhomGia { get; set; }

        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }

        public LoaiGiaNoiGioiThieuHopDong? NhomGiaThuocId { get; set; }
        public LoaiGiaNoiGioiThieuHopDong? NhomGiaVTYTId { get; set; }

        public long? DuocPhamBenhVienId { get; set; }
        public long?  VatTuBenhVienId { get; set; }
      
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


        public decimal? DonGiaNGTTuLan1 { get; set; }
        public decimal? HeSoLan1 { get; set; }
        public decimal? DonGiaNGTTuLan2 { get; set; }
        public decimal? HeSoLan2 { get; set; }
        public decimal? DonGiaNGTTuLan3 { get; set; }
        public decimal? HeSoLan3 { get; set; }
        public decimal? HeSo { get; set; }
    }
}
