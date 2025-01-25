using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.DuocPham
{
    public class DanhSachYeuCauHoanTraDuocPhamGridVo : GridItem
    {
        public string Ma { get; set; }

        public string NguoiYeuCau { get; set; }

        public string KhoHoanTraTu { get; set; }

        public string KhoHoanTraVe { get; set; }

        public DateTime? NgayYeuCau { get; set; }

        public string NgayYeuCauDisplay => NgayYeuCau != null ? NgayYeuCau.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public bool? TinhTrang { get; set; }

        public string TinhTrangDisplay => GetTinhTrang(TinhTrang);

        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }

        public string NgayDuyetDisplay => NgayDuyet != null ? NgayDuyet.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        private string GetTinhTrang(bool? tinhTrang)
        {
            if (tinhTrang == null) return "Đang chờ duyệt";

            return tinhTrang != false ? "Đã duyệt" : "Từ chối duyệt";
        }
    }
}
