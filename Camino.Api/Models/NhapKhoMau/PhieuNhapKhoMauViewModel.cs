using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.NhapKhoMau
{
    public class PhieuNhapKhoMauViewModel : BaseViewModel
    {
        public PhieuNhapKhoMauViewModel()
        {
            NhapKhoMauChiTiets = new List<PhieuNhapKhoMauChiTietViewModel>();
        }

        public string SoPhieu { get; set; }
        public string SoHoaDon { get; set; }
        public string KyHieuHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public Enums.LoaiNguoiGiaoNhan? LoaiNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }
        public string TenNguoiGiao { get; set; }
        public long? NhaThauId { get; set; }
        public string TenNhaThau { get; set; }
        public DateTime? NgayNhap { get; set; }
        public long? NguoiNhapId { get; set; }
        public string TenNguoiNhap { get; set; }
        public bool? DuocKeToanDuyet { get; set; }
        public Enums.TrangThaiNhapKhoMau TrangThai { get; set; }
        public string TenTrangThai => TrangThai.GetDescription();
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string GhiChu { get; set; }

        public bool InPhieu { get; set; }
        public List<PhieuNhapKhoMauChiTietViewModel> NhapKhoMauChiTiets { get; set; }
    }
}
