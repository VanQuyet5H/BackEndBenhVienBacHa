using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruPhieuDieuTriThuocViewModel : BaseViewModel
    {
        public long? KhoId { get; set; }
        public int LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public double? SoLuong { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public string DungSang { get; set; }
        public string DungTrua { get; set; }
        public string DungChieu { get; set; }
        public string DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string GhiChu { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TheTich { get; set; }
        public int? TocDoTruyen { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public double? CachGioTruyenDich { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public bool LaTuTruc { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho { get; set; }
        public bool IsDelete { get; set; }
        public int KhuVuc { get; set; }
        public bool? LaTangSTT { get; set; }
        public int? SoLanTrenVien { get; set; }
        public double? CachGioDungThuoc { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public int? ThoiGianBatDauTiem { get; set; }
        public int? SoLanTrenMui { get; set; }
        public double? CachGioTiem { get; set; }
        public int? SoThuTu { get; set; }
    }

    public class YeuCauTraDuocPhamTuBenhNhanChiTietViewModel : BaseViewModel
    {
        public YeuCauTraDuocPhamTuBenhNhanChiTietViewModel()
        {
            YeuCauDuocPhamBenhViens = new List<YcDuocPhamBvVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public bool LaDichTruyen { get; set; }

        public List<YcDuocPhamBvVo> YeuCauDuocPhamBenhViens { get; set; }
    }

    public class YcDuocPhamBvViewModel : BaseViewModel
    {
        public long YeuCauDuocPhamBenhVienId { get; set; }
        public double? SoLuongTra { get; set; }
    }

}
