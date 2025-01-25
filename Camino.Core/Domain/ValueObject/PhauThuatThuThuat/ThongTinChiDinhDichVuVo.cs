using System;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class ThongTinChiDinhDichVuVo
    {
        public long NhanVienChiDinhId { get; set; }
        public string NhanVienChiDinhDisplay { get; set; }
        public long? NoiChiDinhId { get; set; }
        public string NoiChiDinhDisplay { get; set; }
        public bool? IsDichVuKhongCanTuongTrinh { get; set; }

        //BVHD-3882
        public long? ThuePhongId { get; set; }
        public bool CoThuePhong => ThuePhongId != null;
        public long? CauHinhThuePhongId { get; set; }
        public DateTime? ThoiDiemBatDau { get; set; }
        public DateTime? ThoiDiemKetThuc { get; set; }

        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public bool LaHinhThucDenGioiThieu { get; set; }
        public string BacSiGioiThieu { get; set; }
        public string MessageCanhBaoThuePhong => LaHinhThucDenGioiThieu
            ? ((!string.IsNullOrEmpty(BacSiGioiThieu) ? $"NB do {BacSiGioiThieu} giới thiệu. " : "NB có hình thức đến là giới thiệu. ") + "Lưu ý: Kiểm tra thông tin thuê phòng trước khi kết thúc.")
            : string.Empty;

    }
}
