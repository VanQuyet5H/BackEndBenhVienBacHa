using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems
{
    public class DuyetKetQuaXetNghiemForServiceProcessingVo : GridItem
    {
        public List<DuyetKqXetNghiemChiTietVo> ChiTietKetQuaXetNghiems { get; set; }
        public string ChanDoan { get; set; }
        public string GhiChu { get; set; }

    }

    public class DuyetKqXetNghiemChiTietVo : GridItem
    {
        public int Level { get; set; }

        public long[] IdChilds { get; set; }

        public string GiaTriDuyet { get; set; }

        public bool? ToDamGiaTri { get; set; }

        public string NguoiDuyet { get; set; }

        public bool? Duyet { get; set; }

        public DateTime? ThoiDiemDuyetKetQua { get; set; }

        public string ThoiDiemDuyetKetQuaDisplay => ThoiDiemDuyetKetQua != null ? (ThoiDiemDuyetKetQua ?? DateTime.Now).ApplyFormatDateTime() : string.Empty;
    }

    public class ChildrenInfo : GridItem
    {
        public long YcdvktId { get; set; }

        public long DichVuXetNghiemId { get; set; }
    }

    public class PhienXNGanNhatVo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SearchStringBarCode { get; set; }
    }
}
