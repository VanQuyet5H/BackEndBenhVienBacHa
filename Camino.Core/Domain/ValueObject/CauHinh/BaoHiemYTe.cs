namespace Camino.Core.Domain.ValueObject.CauHinh
{

    public class BaoHiemYTe : ISettings
    {
        public BaoHiemYTe()
        {
            QuanHeNhanThanChaDeId = 3;
            QuanHeNhanThanMeDeId = 4;
        }
        public string BenhVienTiepNhan { get; set; }
        public int GioiHanSoNguoiKham { get; set; }
        public int HangBenhVien { get; set; }
        public long QuanHeNhanThanMeDeId { get; set; }
        public long QuanHeNhanThanChaDeId { get; set; }
        public bool GoiCongBHYT { get; set; }
        public int MaPhuongThucThanhToanXML2 { get; set; }
        public int MaPhuongThucThanhToanXML3 { get; set; }
    }
}