using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.VatTuBenhViens
{
    public class VatTuBenhVienTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public decimal? Gia { get; set; }
    }
}
