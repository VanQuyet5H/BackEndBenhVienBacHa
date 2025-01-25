using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    //public class CauHinhNoiGioiThieuVaHoaHongExportExcel
    //{
    //    public int LaDichVu { get; set; }
    //}
    public class CauHinhNoiGioiThieuDichVuExportExcel
    {
        [Width(50)]
        public string LaDichVu { get; set; }
        [Width(50)]
        public string Ma { get; set; }
        [Width(50)]
        public string TenDichVu { get; set; }
        [Width(50)]
        public string NhomGiaDichVu { get; set; }

        [Width(50)]
        public string DonGia { get; set; }
        [Width(50)]
        public string DonGiaNGTTuLan1 { get; set; }
        [Width(50)]
        public string HeSoLan1 { get; set; }
        [Width(50)]
        public string DonGiaNGTTuLan2 { get; set; }
        [Width(50)]
        public string HeSoLan2 { get; set; }
        [Width(50)]
        public string DonGiaNGTTuLan3 { get; set; }
        [Width(50)]
        public string HeSoLan3 { get; set; }
        [Width(50)]
        public string Ghichu { get; set; }
    }

    public class CauHinhNoiGioiThieuDPVTYTExportExcel
    {

        [Width(50)]
        public int LaDichVu { get; set; }

        [Width(50)]
        public string Ma { get; set; }

        [Width(50)]
        public string Ten { get; set; }

        [Width(50)]
        public string NhomGia { get; set; }

        [Width(50)]
        public string HeSo { get; set; }
        [Width(50)]
        public string Ghichu { get; set; }
    }
    public class CauHinhHoaHongDichVuExportExcel
    {
        [Width(25)]
        public string LaDichVu { get; set; }

        [Width(30)]
        public string Ma { get; set; }

        [Width(40)]
        public string Ten { get; set; }

        [Width(40)]
        public string NhomGia { get; set; }

        [Width(50)]
        public string DonGia { get; set; }

        [Width(50)]
        public string LoaiHoaHong { get; set; }

        [Width(50)]
        public string DonGiaHoaHong { get; set; }

        [Width(20)]
        public string ApDungHoaHongTuLanDisplay { get; set; }

        [Width(20)]
        public string ApDungHoaHongDenLanDisplay { get; set; }
        [Width(50)]
        public string Ghichu { get; set; }
    }

    public class CauHinhHoaHongDPVTYTExportExcel
    {
        [Width(25)]
        public string LaDichVu { get; set; }

        [Width(30)]
        public string Ma { get; set; }

        [Width(40)]
        public string Ten { get; set; }

        [Width(50)]
        public string DonGiaHoaHong { get; set; }


        [Width(50)]
        public string Ghichu { get; set; }
    }
}
