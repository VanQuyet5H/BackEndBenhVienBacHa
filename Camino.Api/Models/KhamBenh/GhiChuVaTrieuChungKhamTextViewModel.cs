namespace Camino.Api.Models.KhamBenh
{
    public class GhiChuVaTrieuChungKhamTextViewModel
    {
        public string TrieuChungKham { get; set; }

        public bool FlagChangeTrieuChungKhamText { get; set; }

        public string GhiChuTrieuChungKham { get; set; }

        public bool FlagChangeGhiChuTrieuChungKhamText { get; set; }

        public long YeuCauTiepNhanId { get; set; }

        public long YeuCauKhamBenhId { get; set; }
    }
}
