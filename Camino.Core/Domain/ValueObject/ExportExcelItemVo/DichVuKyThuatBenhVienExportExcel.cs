using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DichVuKyThuatBenhVienExportExcel
    {
        [Width(30)]
        public string Ma { get; set; }

        [Width(23)]
        public string Ma4350 { get; set; }

        [Width(100)]
        public string Ten { get; set; }

        [Width(110)]
        public string TenNoiThucHien { get; set; }

        [Width(54)]
        public string NgayBatDauHienThi { get; set; }

        [Width(54)]
        public string ThongTu { get; set; }

        [Width(54)]
        public string NghiDinh { get; set; }

        [Width(54)]
        public string NoiBanHanh { get; set; }

        [Width(20)]
        public long? SoMayTT { get; set; }

        [Width(20)]
        public long? SoMayCBCM { get; set; }

        [Width(25)]
        public long? ThoiGianThucHien { get; set; }

        [Width(20)]
        public long? SoCaCP { get; set; }

        [Width(50)]
        public string MoTa { get; set; }

        [Width(20)]
        public string HieuLucHienThi { get; set; }
    }
}
