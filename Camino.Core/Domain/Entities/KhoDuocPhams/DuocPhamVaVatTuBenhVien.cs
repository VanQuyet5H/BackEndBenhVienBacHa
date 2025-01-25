using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.VatTuBenhViens;

namespace Camino.Core.Domain.Entities.KhoDuocPhams
{
    public class DuocPhamVaVatTuBenhVien : BaseEntity
    {
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public bool HieuLuc { get; set; }
    }
}
