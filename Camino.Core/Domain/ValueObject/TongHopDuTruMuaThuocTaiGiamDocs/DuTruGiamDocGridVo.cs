using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaThuocTaiGiamDocs
{
    public class DuTruGiamDocGridVo : GridItem
    {
        public string SoPhieu { get; set; }

        public DateTime TuNgay { get; set; }

        public DateTime DenNgay { get; set; }

        public string KyDuTru { get; set; }

        public string NguoiYeuCau { get; set; }

        public DateTime NgayYeuCau { get; set; }

        public string NgayYeuCauDisplay => NgayYeuCau.ApplyFormatDateTime();

        public bool? TrangThai { get; set; }

        public string TrangThaiDisplay => this.ProcessTrangThai(TrangThai);

        private string ProcessTrangThai(bool? trangThai)
        {
            if (trangThai == null) return "<span class='orange-txt'>Chờ duyệt</span>";
            return trangThai == true ? "<span class='green-txt'>Đã duyệt</span>"
                : "<span class='red-txt'>Từ chối</span>";
        }

        public DateTime? NgayDuyet { get; set; }

        public string NgayDuyetDisplay =>
            NgayDuyet != null ? NgayDuyet.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;
    }
}
