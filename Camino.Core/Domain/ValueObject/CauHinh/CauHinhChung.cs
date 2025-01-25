namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhChung : ISettings
    {
        public CauHinhChung()
        {
            DuyetBHYTTuDong = true;
            UuTienXuatKhoTheoHanSuDung = false;
            ThoiGianGiuDonThuoc = 2;//2 giờ
        }
        public bool DuyetBHYTTuDong { get; set; }
        public bool UuTienXuatKhoTheoHanSuDung { get; set; }
        public int ThoiGianGiuDonThuoc { get; set; }

        public string NgayLamViec { get; set; }
        public int GioBatDauLamViec { get; set; }
        public int GioKetThucLamViec { get; set; }
    }
}
