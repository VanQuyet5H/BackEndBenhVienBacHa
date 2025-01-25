namespace Camino.Api.Models.DuyetPhieuHoanTraThuocTuBns
{
    public class ApproveRequestParam : BaseViewModel
    {
        public string GhiChu { get; set; }
        public bool? LaDichTruyen { get; set; }
        public long HoanTraVeKhoId { get; set; }

    }
}
