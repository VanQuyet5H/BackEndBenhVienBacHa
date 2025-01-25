using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Xml.Linq;
using Camino.Core.Helpers;
using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanBu
{
    public class DanhSachDuocPhamCanBuQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
    }
    public class DanhSachDuocPhamCanBuChiTietQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaBHYT { get; set; }
    }
}
