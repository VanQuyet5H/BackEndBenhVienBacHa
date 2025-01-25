using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ChiPhiDaThanhToanKhamChuaBenhVo : GridItem
    {
        public ChiPhiDaThanhToanKhamChuaBenhVo()
        {
            TaiKhoanBenhNhanChiDetails = new List<TaiKhoanBenhNhanChiDetail>();           
        }

        public NhomChiPhiKhamChuaBenh LoaiNhom { get; set; }
        public string Nhom => LoaiNhom.GetDescription();
        public int STT { get; set; }
        public bool DuocHuongBHYT { get; set; }
        public string Ma { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public double Soluong { get; set; }
        public decimal DonGia { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal ThanhTien => KhongTinhPhi == true ? 0 : (decimal)(Soluong * (double)DonGia);
        public bool KiemTraBHYTXacNhan { get; set; }
        public decimal BHYTThanhToan => (decimal)(Soluong * (double)DonGiaBHYTThanhToan);
        public decimal DonGiaBHYT { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuongBaoHiem { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGiaBHYT * TiLeBaoHiemThanhToan / 100 * MucHuongBaoHiem / 100;
        public int TLMG { get; set; }
        public decimal SoTienMG { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal TongCongNo { get; set; }
        public decimal BNConPhaiThanhToan => ThanhTien - BHYTThanhToan - TongCongNo - SoTienMG - DaThanhToan;
        public string NoiThucHien { get; set; }
        public bool CheckedDefault { get; set; }
        public bool KiemTraHuy { get; set; }
        public List<TaiKhoanBenhNhanChiDetail> TaiKhoanBenhNhanChiDetails { get; set; }
        public string TrangThai { get; set; }


        public IEnumerable<DanhSachMienGiamVo> DanhSachMienGiamVos { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public long? NoiDungGhiChuMiemGiamId { get; set; }
    }

    public class TaiKhoanBenhNhanChiDetail
    {
        public int SoPhieu { get; set; }
        public string ThoiGianThuStr { get; set; }
        public string NguoiThu { get; set; }
    }
}
