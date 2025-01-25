using System;
using System.Collections.Generic;
using Camino.Api.Models.NhapKhoDuocPhamChiTiets;

namespace Camino.Api.Models.HopDongThauDuocPham
{
    public class HopDongThauDuocPhamViewModel : BaseViewModel
    {
        public long NhaThauId { get; set; }

        public string NhaThau { get; set; }

        public string SoHopDong { get; set; }

        public string SoQuyetDinh { get; set; }

        public DateTime? CongBo { get; set; }

        public DateTime? NgayKy { get; set; }

        public DateTime? NgayHieuLuc { get; set; }

        public DateTime? NgayHetHan { get; set; }

        public string CongBoDisplay { get; set; }

        public string NgayKyDisplay { get; set; }

        public string NgayHieuLucDisplay { get; set; }

        public string NgayHetHanDisplay { get; set; }

        public int LoaiThau { get; set; }

        public int? LoaiThuocThau { get; set; }

        public string TenLoaiThau { get; set; }

        public string TenLoaiThuocThau { get; set; }

        public string NhomThau { get; set; }

        public string GoiThau { get; set; }

        public int Nam { get; set; }

        public IList<HopDongThauDuocPhamChiTietViewModel> HopDongThauDuocPhamChiTiets { get; set; }

        public IList<NhapKhoDuocPhamChiTietViewModel> NhapKhoDuocPhamChiTiets { get; set; }
    }
}
