using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class HuyThanhToanThuPhiKhamBenhRequestVo
    {
        public int YeuCauTiepNhanId { get; set; }
        public string LyDoHuy { get; set; }
        public List<ChiPhiKhamChuaBenhVo> DanhSachChiPhiKhamChuaBenhDaChons { get; set; }
    }
}
