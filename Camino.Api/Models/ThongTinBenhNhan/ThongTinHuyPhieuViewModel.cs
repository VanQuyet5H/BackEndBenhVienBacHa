using System;

namespace Camino.Api.Models.ThongTinBenhNhan
{
    public class ThongTinHuyPhieuViewModel : BaseViewModel
    {
        public long SoPhieu { get; set; }
        public Camino.Core.Domain.ValueObject.YeuCauTiepNhans.LoaiPhieuThuChiThuNgan LoaiPhieuThuChiThuNgan { get; set; }
        public bool? ThuHoiPhieu { get; set; }
        public int? NguoiThuHoiId { get; set; }
        public string TenNguoiThuHoi { get; set; }
        public DateTime? ThoiGianThuHoi { get; set; }
        public string LyDo { get; set; }

        public bool? HuyPhieuHoanUng { get; set; }
        public bool? ChuyenTienQuaTamUng { get; set; }
        public bool? KiemTraThuHoi { get; set; }
    }
}