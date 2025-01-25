using Camino.Core.Domain;
using System.Collections.Generic;

namespace Camino.Api.Models.BHYT
{
    public class BhytNoiTruChiTietViewModel : BaseViewModel
    {
        public BhytNoiTruChiTietViewModel()
        {
            ThongTinBhytNoiTrus = new List<ThongTinBhytNoiTruViewModel>();
        }

        public string MaYeuCauTiepNhan { get; set; }

        public string MaBn { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        public string Email { get; set; }

        public string DoiTuongUuDai { get; set; }

        public string CongTyUuDai { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string LyDoVaoVien { get; set; }

        public string GiayChuyenVien { get; set; }

        public long? GiayChuyenVienId { get; set; }

        public List<ThongTinBhytNoiTruViewModel> ThongTinBhytNoiTrus { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class ThongTinBhytNoiTruViewModel : BaseViewModel
    {
        public string BhytMaSoThe { get; set; }

        public string BhytNgayHieuLucStr { get; set; }

        public string BhytNgayHetHanStr { get; set; }

        public string Dkbd { get; set; }

        public string GiayMienCungChiTra { get; set; }

        public long? GiayMienCungChiTraId { get; set; }

        public int? BhytMucHuong { get; set; }
    }
}
