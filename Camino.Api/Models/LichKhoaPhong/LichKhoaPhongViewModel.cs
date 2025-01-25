using Camino.Core.Domain;
using System;
using System.Collections.Generic;

namespace Camino.Api.Models.LichKhoaPhong
{


    public class NhanVienLichPhanCongViewModel : BaseViewModel
    {

        public long PhongBenhVienId { get; set; }

        public long NhanVienId { get; set; }

        public string TenNhanVien { get; set; }

        public DateTime NgayPhanCong { get; set; }

        public Enums.EnumLoaiThoiGianPhanCong LoaiThoiGianPhanCong { get; set; }

        public bool IsBacSi { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public int Thu { get; set; }
        public bool IsDeleteHiden { get; set; }
        public List<long> ListIdValidator { get; set; }

    }
    public class LichKhoaPhongViewModel : BaseViewModel
    {
        public LichKhoaPhongViewModel()
        {
            ThoiGianTrucNhanVienBuoiSangs = new ThoiGianTrucNhanVienModel();
            ThoiGianTrucNhanVienBuoiChieus = new ThoiGianTrucNhanVienModel();
            NhanVienTrucDelete = new List<NhanVienLichPhanCongViewModel>();
        }
        public long PhongBenhVienId { get; set; }
        public string TenKhoa { get; set; }
        public string TenPhong { get; set; }

        public ThoiGianTrucNhanVienModel ThoiGianTrucNhanVienBuoiSangs { get; set; }
        public ThoiGianTrucNhanVienModel ThoiGianTrucNhanVienBuoiChieus { get; set; }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsCopy { get; set; }
        public int CopyForWeek { get; set; }
        public string CaTruc { get; set; }
        public Enums.EnumLoaiThoiGianPhanCong CaTrucInt { get; set; }
        public List<NhanVienLichPhanCongViewModel> BacSis { get; set; }
        public List<NhanVienLichPhanCongViewModel> YTas { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string HostingPrint { get; set; }
        public bool IsShowCopyForWeek { get; set; }
    }
    public class ThoiGianTrucNhanVienModel
    {
        public ThoiGianTrucNhanVienModel()
        {
            NhanVienTrucT2 = new List<NhanVienLichPhanCongViewModel>();
            NhanVienTrucT3 = new List<NhanVienLichPhanCongViewModel>();
            NhanVienTrucT4 = new List<NhanVienLichPhanCongViewModel>();
            NhanVienTrucT5 = new List<NhanVienLichPhanCongViewModel>();
            NhanVienTrucT6 = new List<NhanVienLichPhanCongViewModel>();
            NhanVienTrucT7 = new List<NhanVienLichPhanCongViewModel>();
            NhanVienTrucT8 = new List<NhanVienLichPhanCongViewModel>();
        }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucT2 { get; set; }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucT3 { get; set; }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucT4 { get; set; }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucT5 { get; set; }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucT6 { get; set; }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucT7 { get; set; }
        public List<NhanVienLichPhanCongViewModel> NhanVienTrucT8 { get; set; }



    }
    public class PhongsViewModel : BaseViewModel
    {
        public PhongsViewModel()
        {
            Phong = new List<LichKhoaPhongViewModel>();
        }
        public List<LichKhoaPhongViewModel> Phong { get; set; }
        public int Index { get; set; }
    }
    public class PrintWeek
    {      
        public string TenKhoa { get; set; }
        public string HostName { get; set; }
        public string ToDateFromDate { get; set; }
        public string NgayThu2 { get; set; }
        public string NgayThu3 { get; set; }
        public string NgayThu4 { get; set; }
        public string NgayThu5 { get; set; }
        public string NgayThu6 { get; set; }
        public string NgayThu7 { get; set; }
        public string NgayThu8 { get; set; }
    }
}
