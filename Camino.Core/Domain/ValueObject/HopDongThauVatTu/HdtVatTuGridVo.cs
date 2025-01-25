using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.HopDongThauVatTu
{
    public class HdtVatTuGridVo : GridItem
    {
        public string NhaThau { get; set; }

        public string SoHopDong { get; set; }

        public string SoQuyetDinh { get; set; }

        public DateTime? CongBo { get; set; }

        public string CongBoDisplay => CongBo.GetValueOrDefault().ApplyFormatDate();

        public DateTime? NgayKy { get; set; }

        public string NgayKyDisplay => NgayKy!=null?NgayKy.GetValueOrDefault().ApplyFormatDate():"";

        public DateTime? NgayHl { get; set; }

        public string NgayHieuLucDisplay => NgayHl.GetValueOrDefault().ApplyFormatDate();

        public DateTime? NgayHh { get; set; }

        public string NgayHetHanDisplay => NgayHh.GetValueOrDefault().ApplyFormatDate();

        public Enums.EnumLoaiThau? LoaiThau { get; set; }

        public string TenLoaiThau => LoaiThau.GetValueOrDefault().GetDescription();

        public string NhomThau { get; set; }

        public string GoiThau { get; set; }

        public int? Nam { get; set; }
    }
}
