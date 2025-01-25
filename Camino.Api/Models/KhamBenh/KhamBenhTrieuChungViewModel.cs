namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhTrieuChungViewModel
    {
        public long[] idTrieuChungsInsert { get; set; }

        public long[] idTrieuChungsDelete { get; set; }

        public long idYeuCauKhamBenh { get; set; }

        public bool trieuChungChange { get; set; }
    }
}
