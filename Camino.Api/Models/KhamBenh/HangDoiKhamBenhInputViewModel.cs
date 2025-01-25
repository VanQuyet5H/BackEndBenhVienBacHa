namespace Camino.Api.Models.KhamBenh
{
    public class HangDoiKhamBenhInputViewModel
    {
        public long? HangDoiDangKhamId { get; set; }
        public long HangDoiBatDauKhamId { get; set; }
        public bool HoanThanhKham { get; set; }
        public long? PhongBenhVienId { get; set; }
        public bool LaKhamDoan { get; set; }
    }
}
