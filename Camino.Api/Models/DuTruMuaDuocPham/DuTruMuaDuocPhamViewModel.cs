using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DuTruMuaDuocPham
{
    public class DuTruMuaDuocPhamViewModel : BaseViewModel
    {
        public DuTruMuaDuocPhamViewModel()
        {
            DuTruMuaDuocPhamChiTiets = new List<DuTruMuaDuocPhamChiTietViewModel>();
        }
        public string SoPhieu { get; set; }
        public EnumNhomDuocPhamDuTru? NhomDuocPhamDuTru { get; set; }
        public string TenNhomDuTru { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long? KyDuTruMuaDuocPhamVatTuId { get; set; }
        public string TenKyDuTru { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string GhiChu { get; set; }
        public bool? TruongKhoaDuyet { get; set; }
        public long? TruongKhoaId { get; set; }
        public string TenTruongKhoa { get; set; }
        public long? NhanVienKhoDuocId { get; set; }
        public string TenNhanVienKhoDuoc { get; set; }
        public long? GiamDocId { get; set; }
        public string TenGiamDoc { get; set; }
        public DateTime? NgayTruongKhoaDuyet { get; set; }
        public DateTime? NgayKhoDuocDuyet { get; set; }
        public DateTime? NgayGiamDocDuyet { get; set; }

        public long? DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public string LyDoTruongKhoaTuChoi { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocId { get; set; }
        public List<DuTruMuaDuocPhamChiTietViewModel> DuTruMuaDuocPhamChiTiets { get; set; }

    }
}
