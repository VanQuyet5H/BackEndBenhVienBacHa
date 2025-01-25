using System;
using System.Collections.Generic;

namespace Camino.Api.Models.YeuCauTraDuocPhamTuBenhNhan
{
    public class YeuCauTraDuocPhamTuBenhNhanViewModel : BaseViewModel
    {
        public YeuCauTraDuocPhamTuBenhNhanViewModel()
        {
            YeuCauTraThuocTuBenhNhanChiTiets = new List<TraDuocPhamTuBenhNhanChiTietViewModel>();
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
        public List<TraDuocPhamTuBenhNhanChiTietViewModel> YeuCauTraThuocTuBenhNhanChiTiets { get; set; }
    }
    public class TraDuocPhamTuBenhNhanChiTietViewModel
    {
        public long YeuCauDuocPhamBenhVienId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
    }

}
