using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ChiPhiKhamChuaBenhCoCongNoVo : GridItem
    {
        public NhomChiPhiKhamChuaBenh LoaiNhom { get; set; }
        public string Nhom => LoaiNhom.GetDescription();
        public int STT { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)(Soluong * (double)DonGia);
        public bool KiemTraBHYTXacNhan { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public decimal BHYTThanhToan => (decimal)(Soluong * (double)DonGiaBHYTThanhToan);
        public decimal DonGiaBHYTThanhToan { get; set; }
        public int TLMG { get; set; }
        public decimal SoTienMG => (decimal)((Soluong * (double)DonGia - (double)BHYTThanhToan) * TLMG / 100);
        public decimal DaThanhToan { get; set; }
        public decimal BNConPhaiThanhToan => (decimal)((Soluong * (double)DonGia - (double)BHYTThanhToan) - ((Soluong * (double)DonGia - (double)BHYTThanhToan) * TLMG / 100) - (double)DaThanhToan);
        public string NoiThucHien { get; set; }
        //Biến này em dùng checkUi CheckedDefault = flalse 
        public bool CheckedDefault { get; set; }

        public List<DanhSachCongNoChoThu> DanhSachCongNoChoThus { get; set; }
    }
    public class DanhSachCongNoChoThu
    {
        public int CongNoId { get; set; }
        public decimal SoTienCongNoChoThu { get; set; }
    }
}
