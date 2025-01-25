using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo : QueryInfo
    {
        public long KhoId { get; set; }
        public long DuocPhamHoacVatTuBenhVienId { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class LookupItemDuocPhamHoacVatTuVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public long DuocPhamHoacVatTuBenhVienId { get; set; }

        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
    }
}
