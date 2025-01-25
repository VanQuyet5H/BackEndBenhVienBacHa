using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoThongKeDonThuoc
{
    public class BaoCaoThongKeDonThuocGridVo : GridItem
    {
        public int STT { get; set; }
        public string MaYT { get; set; }
        public string SoBenhAn { get; set; }
        public string HoVaTen { get; set; }
        public int NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string MaBHYT { get; set; }
        public string MaDKBD { get; set; }
        public string MaBenh { get; set; }
        public DateTime? NgayVao { get; set; }
        public string NgayVaoString => NgayVao != null ? NgayVao.Value.ApplyFormatDate() : "";
        public DateTime? NgayRa { get; set; }
        public string NgayRaString => NgayRa != null ? NgayRa.Value.ApplyFormatDate() : "";
        public string ChanDoan { get; set; }
        public string TienSuBenh { get; set; }
        public string KhoaRa { get; set; }
        public string TrangThai { get; set; }
        public string BsKeToa { get; set; }
        public string TenThuoc { get; set; }
        public string HamLuong { get; set; }
        public double SoLuong { get; set; }
        public double Sang { get; set; }
        public double Trua { get; set; }
        public double Chieu { get; set; }
        public double Toi { get; set; }
        public double Tra { get; set; }
        public string GhiChu { get; set; }
        public string KhoPhat { get; set; }
        public string BHYT { get; set; }
        public DateTime NgayDuyetPhieu { get; set; }
        public string NgayDuyetPhieuString => NgayDuyetPhieu.ApplyFormatDate();
    }
}
