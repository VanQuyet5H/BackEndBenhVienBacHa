using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Xml.Linq;
using Camino.Core.Helpers;
using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanLinhTrucTiep
{
    public class DanhSachDuocPhamCanLinhTrucTiepQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }
    }
    public class DanhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        //public bool? LaBHYT { get; set; }
    }
}
