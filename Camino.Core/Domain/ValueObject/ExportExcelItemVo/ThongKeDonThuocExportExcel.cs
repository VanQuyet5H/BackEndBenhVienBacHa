using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ThongKeDonThuocExportExcel 
    {
        [Width(20)]
        public int STT { get; set; }
        [Width(40)]
        public string MaYT { get; set; }
        [Width(40)]
        public string SoBenhAn { get; set; }
        [Width(100)]
        public string HoVaTen { get; set; }
        [Width(40)]
        public int NamSinh { get; set; }
        [Width(40)]
        public string GioiTinh { get; set; }
        [Width(40)]
        public string DiaChi { get; set; }
        [Width(150)]
        public string MaBHYT { get; set; }
        [Width(40)]
        public string MaDKBD { get; set; }
        [Width(40)]
        public string MaBenh { get; set; }
       
        [Width(40)]
        public string NgayVaoString { get; set; }
        [Width(40)]
        public string NgayRaString { get; set; }
        [Width(100)]
        public string ChanDoan { get; set; }
        [Width(50)]
        public string TienSuBenh { get; set; }
        [Width(40)]
        public string KhoaRa { get; set; }
        [Width(40)]
        public string TrangThai { get; set; }
        [Width(80)]
        public string BsKeToa { get; set; }
        [Width(120)]
        public string TenThuoc { get; set; }
        [Width(40)]
        public string HamLuong { get; set; }
        [Width(40)]
        public double SoLuong { get; set; }
        [Width(40)]
        public double Sang { get; set; }
        [Width(40)]
        public double Trua { get; set; }
        [Width(40)]
        public double Chieu { get; set; }
        [Width(40)]
        public double Toi { get; set; }
        [Width(40)]
        public double Tra { get; set; }
        [Width(40)]
        public string GhiChu { get; set; }
        [Width(40)]
        public string KhoPhat { get; set; }
        [Width(40)]
        public string BHYT { get; set; }
        [Width(40)]
        public string NgayDuyetPhieuString { get; set; }
    }
}
