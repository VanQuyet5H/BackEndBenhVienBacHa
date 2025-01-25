using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.Cauhinh
{
    public class CauHinhDanhSachChiTietViewModel
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public string GhiChu { get; set; }
        public Enums.DataType? DataType { get; set; }
        public bool IsDisabled { get; set; }
    }
}
