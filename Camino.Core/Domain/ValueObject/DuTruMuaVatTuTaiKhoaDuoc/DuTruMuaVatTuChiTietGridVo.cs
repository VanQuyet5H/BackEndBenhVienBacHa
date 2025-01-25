using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoaDuoc
{
    
    public class DuTruMuaVatTuChiTietGridVo : GridItem
    {
        public long DuTruMuaVatTuId { get; set; }
        public long VatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
        //public EnumNhomDieuTriDu? NhomDieuTriDuPhong { get; set; }
        public long? DuTruMuaVatTuTheoKhoaChiTietId { get; set; }
        public long? DuTruMuaVatTuKhoDuocChiTietId { get; set; }
    }
    public class DuTruMuaVatTuKhoaChiTietGridVo : GridItem
    {
        public long DuTruMuaVatTuTheoKhoaId { get; set; }
        public long VatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public int? SoLuongDuTruKhoDuocDuyet { get; set; }
        public long? DuTruMuaVatTuKhoDuocChiTietId { get; set; }
    }
}
