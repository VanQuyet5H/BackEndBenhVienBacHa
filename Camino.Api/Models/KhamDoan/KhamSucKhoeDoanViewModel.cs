using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamDoan
{
    public class GoiKhamSucKhoeViewModel : BaseViewModel
    {
        public GoiKhamSucKhoeViewModel()
        {
            DichVuKhamSucKhoeDoans = new List<GoiKhamDichVuKhamSucKhoeDoanViewModel>();
            GoiKhamSucKhoeDichVuKhamBenhs = new List<GoiKhamDichVuKhamSucKhoeDoanViewModel>();
            GoiKhamSucKhoeDichVuDichVuKyThuats = new List<GoiKhamDichVuKhamSucKhoeDoanViewModel>();
        }
        public long? CongTyKhamSucKhoeId { get; set; }
        public string TenCongTy { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public string SoHopDong { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public bool? IsKetThucHopDong { get; set; }
        public bool? GoiChung { get; set; }
        public bool? GoiDichVuPhatSinh { get; set; }
        public bool? CoSuDungGoiChung { get; set; }
        public bool? IsCopy { get; set; }
        public List<GoiKhamDichVuKhamSucKhoeDoanViewModel> DichVuKhamSucKhoeDoans { get; set; }
        public List<GoiKhamDichVuKhamSucKhoeDoanViewModel> GoiKhamSucKhoeDichVuKhamBenhs { get; set; }
        public List<GoiKhamDichVuKhamSucKhoeDoanViewModel> GoiKhamSucKhoeDichVuDichVuKyThuats { get; set; }

    }

    public class GoiKhamDichVuKhamSucKhoeDoanViewModel : BaseViewModel
    {
        public GoiKhamDichVuKhamSucKhoeDoanViewModel()
        {
            GoiKhamSucKhoeNoiThucHiens = new List<GoiKhamSucKhoeNoiThucHienViewModel>();
            GoiKhamSucKhoeNoiThucHienIds = new List<long>();
            DichVuKhamBenhIds = new List<long>();
            DichVuKyThuatIds = new List<long>();
        }

        public string Ma { get; set; }
        public string Ten { get; set; }
        public bool LaDichVuKham { get; set; }
        public NhomDichVuChiDinhKhamSucKhoe? Nhom { get; set; }
        //Nhom : 1 => Kham benh, 2 => Xet nghiem, 3 => Chuan doan hinh anh, 4 => tham do chuc nang
        public string TenNhom => Nhom.GetDescription();
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public string LoaiGia { get; set; } 
        public string TenDichVuKyThuatBenhVien { get; set; }
        public decimal? DonGiaBenhVien { get; set; }

        //update BVHD-3944
        public decimal? DonGiaUuDai { get; set; }
        public decimal? DonGiaThucTe { get; set; }
        //update end BVHD-3944

        public decimal? DonGiaChuaUuDai { get; set; }
        public HinhThucKhamBenh HinhThucKhamBenh { get; set; }
        public bool? GioiTinhNam { get; set; }
        public bool? GioiTinhNu { get; set; }
        public string GioiTinh
        {
            get
            {
                var lstGioiTinh = new List<string>();
                if (GioiTinhNam == true)
                {
                    lstGioiTinh.Add(LoaiGioiTinh.GioiTinhNam.GetDescription());
                }
                if (GioiTinhNu == true)
                {
                    lstGioiTinh.Add(LoaiGioiTinh.GioiTinhNu.GetDescription());
                }
                return string.Join(", ", lstGioiTinh);
            }
        }
        public bool? CoMangThai { get; set; }
        public bool? KhongMangThai { get; set; }
        public string MangThai => CoMangThai == true ? "Có" : (KhongMangThai == true ? "Không" : "");
        public bool? DaLapGiaDinh { get; set; }
        public bool? ChuaLapGiaDinh { get; set; }
        public string TinhTrangHonNhan => DaLapGiaDinh == true ? "Đã lập gia đình" : (ChuaLapGiaDinh == true ? "Chưa lập gia đình" : "");
        public int? SoTuoiTu { get; set; }
        public int? SoTuoiDen { get; set; }
        public int? SoLan { get; set; }
        public string SoTuoiDisplay => SoTuoiTu != null && SoTuoiDen != null ? SoTuoiTu + " - " + SoTuoiDen : (SoTuoiTu != null && SoTuoiDen == null ? "> " + SoTuoiTu : (SoTuoiTu == null && SoTuoiDen != null) ? "≤ " + SoTuoiDen : "");
        public string NoiThucHienString { get; set; }
        public string MaNhomDichVuBenhVien { get; set; }
        public string MaNhomDichVuBenhVienCha { get; set; }
        public bool? GoiChung { get; set; }
        public List<long> GoiKhamSucKhoeNoiThucHienIds { get; set; }

        public List<GoiKhamSucKhoeNoiThucHienViewModel> GoiKhamSucKhoeNoiThucHiens { get; set; }
        public List<long> DichVuKhamBenhIds { get; set; }
        public List<long> DichVuKyThuatIds { get; set; }

    }

    public class GoiKhamSucKhoeNoiThucHienViewModel : BaseViewModel
    {
        public long? GoiKhamSucKhoeDichVuKhamBenhId { get; set; }
        public long? GoiKhamSucKhoeDichVuDichVuKyThuatId { get; set; }
        public long PhongBenhVienId { get; set; }
    }

}
