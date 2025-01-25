namespace Camino.Api.Models.DuocPhamBenhVienPhanNhoms
{
    public class DuocPhamBenhVienPhanNhomViewModel : BaseViewModel
    {
        public string Ten { get; set; }

        public string TenCha { get; set; }

        public long? NhomChaId { get; set; }

        public int? CapNhom { get; set; }

        //BVHD-3454
        public string KyHieuPhanNhom { get; set; }
    }
}
