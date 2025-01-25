using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel : BaseViewModel
    {
        public DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel()
        {
            NoiTruChiDinhDuocPhams = new List<PhaThuocTiemBenhVienChiTietVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public int? SoLanTrenNgay { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TheTich { get; set; }
        public bool LaTuTruc { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool IsDelete { get; set; }
        public int KhuVuc { get; set; }
        public bool? LaTangSTT { get; set; }
        public int? ThoiGianBatDauTiem { get; set; }
        public int? SoLanTrenVien { get; set; }
        public int? SoLanTrenMui { get; set; }
        public double? CachGioTiem { get; set; }
        public string GhiChu { get; set; }
        public double? CachGioDungThuoc { get; set; }
        public int? SoThuTu { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public List<PhaThuocTiemBenhVienChiTietVo> NoiTruChiDinhDuocPhams { get; set; }
    }
}
