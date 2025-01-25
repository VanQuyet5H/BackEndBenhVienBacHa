using System;
using System.Collections.Generic;
using Camino.Core.Domain;

namespace Camino.Api.Models.DichVuKyThuatBenhVien
{
    public class DichVuKyThuatBenhVienViewModel : BaseViewModel
    {
        public DichVuKyThuatBenhVienViewModel()
        {
            DichVuKyThuatVuBenhVienGiaBenhViens = new List<DichVuKyThuatVuBenhVienGiaBenhVienViewModel>();
            DichVuKyThuatBenhVienGiaBaoHiems = new List<DichVuKyThuatBenhVienGiaBaoHiemViewModel>();
            KhoaPhongIds = new List<long>();
            NoiThucHienIds = new List<string>();
            DinhMucDuocPhamVTYTTrongDichVus = new List<DinhMucDuocPhamVTYTTrongDichVuViewModel>();
        }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? DichVuKyThuatId { get; set; }
        public long? KhoaPhongId { get; set; }

        public long? NhomDichVuBenhVienId { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }

        public List<long> KhoaPhongIds { get; set; }
        public List<string> NoiThucHienIds { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string NoiBanHanh { get; set; }
        public int? SoMayThucHien { get; set; }
        public int? SoCanBoChuyenMon { get; set; }
        public int? ThoiGianThucHien { get; set; }
        public int? SoCaChophep { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public bool? CoUuDai { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }
        public string KhoaModelText { get; set; }
        public string DichVuKyThuatModelText { get; set; }
        public bool? AnhXa { get; set; }
        public long? NoiThucHienUuTienId { get; set; }
        public string TenNoiThucHienUuTien { get; set; }
        public int? LoaiPhauThuatThuThuatId { get; set; }
        public string LoaiPhauThuatThuThuat { get; set; }
        public string TenKyThuat { get; set; }
        public Enums.EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }
        public bool? LaVacxin { get; set; }
        public long? NhomDichVuVacxinId { get; set; }

        public bool? CoInKetQuaKemHinhAnh { get; set; }
        public bool? DichVuCoKetQuaLau { get; set; }
        public bool? DichVuKhongKetQua { get; set; }

        public long? ChuyenKhoaChuyenNganhId { get; set; }
        public string ChuyenKhoaChuyenNganhText { get; set; }
        public int? SoPhimXquang { get; set; }

        public bool? DichVuChuyenGoi { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        // BVHD-3961
        public long? KhoaPhongTinhDoanhThuId { get; set; }

        public DichVuKyThuatViewModel DichVuKyThuat { get; set; }
        public DichVuKyThuatBenhVienTiemChungViewModel DichVuKyThuatBenhVienTiemChung { get; set; }
        public List<DichVuKyThuatVuBenhVienGiaBenhVienViewModel> DichVuKyThuatVuBenhVienGiaBenhViens { get; set; }
        public List<DichVuKyThuatBenhVienGiaBaoHiemViewModel> DichVuKyThuatBenhVienGiaBaoHiems { get; set; }
        public List<DinhMucDuocPhamVTYTTrongDichVuViewModel> DinhMucDuocPhamVTYTTrongDichVus { get; set; }
    }
    public class DinhMucDuocPhamVTYTTrongDichVuViewModel :BaseViewModel
    {
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public double? SoLuong{ get; set; }
        public bool? KhongTinhPhi { get; set; }
        public int? LaDuocPham { get; set; }
    }
}