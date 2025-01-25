using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class LichSuTiepNhanKhamSucKhoeDoanExportExcel
    {
        public string TenHopDong { get; set; }
        public string TenCongTy { get; set; }
        public int SoLuongBenhNhan { get; set; }
        public int SoBenhNhanDaDen { get; set; }
        public string NgayBatDauKhamDisplay { get; set; }
        public string NgayKetThucKhamDisplay { get; set; }
        public long IdHopDong { get; set; }
        public long IdCongTy { get; set; }
    }
}
