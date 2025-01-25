using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan
{
    public class DanhSachThuNganNoiTruDaQuyetToanGridVo : GridItem
    {
        public string MaBenhAn { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }

        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhStr => GioiTinh?.GetDescription();

        public string DienThoai { get; set; }
        public string DienThoaiStr { get; set; }
        //public string DoiTuong { get; set; }
        public string KhoaKham { get; set; }
        public decimal SoTienDuTaiKhoan { get; set; }

        public bool CongNo { get; set; }
        public bool DaQuyetToan { get; set; }

        public int MucHuong { get; set; }
        public string DoiTuong => MucHuong > 0 ? "BHYT (" + MucHuong + "%)" : "Viện phí";

        //public TrangThaiDaQuyetToanNoiTru TrangThai => SoTienDuTaiKhoan < 0 ? 
        //       TrangThaiDaQuyetToanNoiTru.CongNo : TrangThaiDaQuyetToanNoiTru.DaQuyetToan;


        public string SearchString { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }

        public DateTime? NgayVaoVien { get; set; }
        public string NgayVaoVienStr => NgayVaoVien?.ApplyFormatDateTimeSACH();
        public DateTime? NgayRaVien { get; set; }
        public string NgayRaVienStr => NgayRaVien?.ApplyFormatDateTimeSACH();


        public string ToDate { get; set; }
        public string FromDate { get; set; }        
    }

    public enum TrangThaiDaQuyetToanNoiTru
    {
        [Description("Công nợ")]
        CongNo = 1,     
        [Description("Đã quyết toán")]
        DaQuyetToan = 2,
    }
}