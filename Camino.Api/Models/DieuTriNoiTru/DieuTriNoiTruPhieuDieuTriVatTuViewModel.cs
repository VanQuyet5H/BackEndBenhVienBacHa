using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruPhieuDieuTriVatTuViewModel : BaseViewModel
    {
        public string Ids { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? KhoId { get; set; }
        public int LaVatTuBHYT { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public double? SoLuong { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool LaTuTruc { get; set; }

    }

    public class YeuCauTraVatTuTuBenhNhanChiTietViewModel : BaseViewModel
    {
        public YeuCauTraVatTuTuBenhNhanChiTietViewModel()
        {
            YeuCauVatTuBenhViens = new List<YcVTBvVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public double? SoLuongTra { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaTra { get; set; }
        public List<YcVTBvVo> YeuCauVatTuBenhViens { get; set; }
    }
}
