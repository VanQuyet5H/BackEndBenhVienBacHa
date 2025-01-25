using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo : GridItem
    {
        public string TenVTDV { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal TongTien => SoLuong * DonGia;

        public long? LoaiVTDVId { get; set; }
        public string TenLoaiVTDV { get; set; }
    }
}
