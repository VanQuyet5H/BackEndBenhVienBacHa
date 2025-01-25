using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class PhieuChamSocSoSinhViewModel : BaseViewModel
    {
        public PhieuChamSocSoSinhViewModel()
        {
            ThongTinHoSoPhieuChamSocSoSinhs = new List<ThongTinHoSoPhieuChamSocSoSinhViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? ICDId { get; set; }
        public string TenICD { get; set; }
        public List<ThongTinHoSoPhieuChamSocSoSinhViewModel> ThongTinHoSoPhieuChamSocSoSinhs { get; set; }
    }

    public class ThongTinHoSoPhieuChamSocSoSinhViewModel
    {
        public DateTime? Ngay { get; set; }
        public int? SLAn { get; set; }
        public bool? NonCho { get; set; }
        public bool? NuocTieu { get; set; }
        public bool? Phan { get; set; }
        public bool? VangDa { get; set; }
        public string XaTriChamSocDanhGia { get; set; }
        public long? NhanVienDieuDuongId { get; set; }
        public string TenNhanVienDieuDuong { get; set; }
    }
}
