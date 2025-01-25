using System;

namespace Camino.Api.Models.DuyetPhieuHoanTraThuocTuBns
{
    public class DuyetPhieuHoanTraThuocTuBnViewModel : BaseViewModel
    {
        public string SoPhieu { get; set; }

        public long? KhoaHoanTraId { get; set; }

        public string KhoaHoanTraDisplay { get; set; }

        public long? HoanTraVeKhoId { get; set; }

        public string HoanTraVeKhoDisplay { get; set; }

        public long? NguoiYeuCauId { get; set; }

        public string NguoiYeuCauDisplay { get; set; }

        public DateTime? NgayYeuCau { get; set; }

        public string GhiChu { get; set; }
        public bool? LaDichTruyen { get; set; }
        public bool? TinhTrang { get; set; }

        public string TinhTrangDisplay => TinhTrang == true ? "<span class='green-txt'>Đã duyệt</span>" : "<span class='orange-txt'>Chờ duyệt</span>";
    }
}
