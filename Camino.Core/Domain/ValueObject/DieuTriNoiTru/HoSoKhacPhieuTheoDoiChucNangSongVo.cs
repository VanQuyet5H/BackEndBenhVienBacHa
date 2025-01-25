namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class HoSoKhacPhieuTheoDoiChucNangSongVo
    {
        public string ChanDoan { get; set; }
    }

    public class HoSoKhacPhieuInTheoDoiChucNangSong
    {
        public string Khoa { get; set; }
        public string SoVaoVien { get; set; }
        public string HoTen { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string SoGiuong { get; set; }
        public string Buong { get; set; }
        public string ChanDoan { get; set; }
        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
    }
}
