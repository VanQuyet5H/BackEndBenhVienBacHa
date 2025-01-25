using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems
{
    public class DuyetKetQuaXetNghiemDetailGridVo : GridItem
    {
        public string NhomXetNghiemDisplay { get; set; }

        public long NhomXetNghiemId { get; set; }

        public string MaDv { get; set; }

        public string TenDv { get; set; }

        public DateTime ThoiGianChiDinh { get; set; }

        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh.ApplyFormatDateTime();

        public string NguoiChiDinh { get; set; }

        public string BenhPham { get; set; }

        public string LoaiMau { get; set; }

        public bool? YeuCauChayLai { get; set; }

        public bool? DaDuyet { get; set; }

        public List<string> DanhSachLoaiMau { get; set; }

        public List<string> DanhSachLoaiMauTongCong { get; set; }

        public List<string> DanhSachLoaiMauKhongDat { get; set; }
    }
}
