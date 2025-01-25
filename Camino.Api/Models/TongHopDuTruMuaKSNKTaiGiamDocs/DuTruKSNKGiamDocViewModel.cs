using System;

namespace Camino.Api.Models.TongHopDuTruMuaKSNKTaiGiamDocs
{
    public class DuTruKSNKGiamDocViewModel : BaseViewModel
    {
        public string SoPhieu { get; set; }

        public string KyDuTru { get; set; }

        public string NguoiYeuCau { get; set; }

        public string LyDoGiamDocTuChoi { get; set; }

        public DateTime NgayYeuCau { get; set; }

        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }

        public string GhiChu { get; set; }

        public bool? TrangThai { get; set; }

        public string TrangThaiDisplay => ProcessTrangThai(TrangThai);

        private string ProcessTrangThai(bool? trangThai)
        {
            if (trangThai == null) return "<span class='orange-txt'>Đang chờ duyệt</span>";
            return trangThai == true ? "<span class='green-txt'>Đã duyệt</span>"
                : "<span class='red-txt'>Từ chối duyệt</span>";
        }
    }
}
