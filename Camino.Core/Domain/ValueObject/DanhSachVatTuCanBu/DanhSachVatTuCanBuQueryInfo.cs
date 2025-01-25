using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Xml.Linq;
using Camino.Core.Helpers;
using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Camino.Core.Domain.ValueObject.DanhSachVatTuCanBu
{
    public class DanhSachVatTuCanBuQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
    }
    public class DanhSachVatTuCanBuChiTietQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool? LaBHYT { get; set; }
    }
}
