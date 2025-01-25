namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhChuanDoanViewModel
    {
        public long[] idChuanDoansInsert { get; set; }

        public long[] idChuanDoansDelete { get; set; }

        public long idYeuCauKhamBenh { get; set; }

        public bool chuanDoanChange { get; set; }
    }
}
