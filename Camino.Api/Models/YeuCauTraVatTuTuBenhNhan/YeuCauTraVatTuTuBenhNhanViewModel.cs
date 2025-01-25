using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauTraVatTuTuBenhNhan
{
    public class YeuCauTraVatTuTuBenhNhanViewModel : BaseViewModel
    {
        public YeuCauTraVatTuTuBenhNhanViewModel()
        {
            YeuCauTraVatTuTuBenhNhanChiTiets = new List<TraVatTuTuBenhNhanChiTietViewModel>();
        }
        public string SoPhieu { get; set; }
        public string TenKhoaTra { get; set; }
        public long? KhoTraId { get; set; }
        public long? KhoaHoanTraId { get; set; }
        public string TenKhoTra { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string GhiChu { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public List<TraVatTuTuBenhNhanChiTietViewModel> YeuCauTraVatTuTuBenhNhanChiTiets { get; set; }
    }
    public class TraVatTuTuBenhNhanChiTietViewModel
    {
        public long YeuCauVatTuBenhVienId { get; set; }
        public long VatTuBenhVienId { get; set; }
    }
}
