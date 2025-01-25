using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems
{
    public class DuyetKetQuaXetNghiemGridVo : GridItem
    {
        public string Barcode { get; set; }

        public string BarcodeNumber { get; set; }
        public long YeuCauTiepNhanId { get; set; }

        public string MaTn { get; set; }

        public string MaBn { get; set; }

        public string HoTen { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string GioiTinhDisplay => ShowGioiTinhTxt(GioiTinh);

        private string ShowGioiTinhTxt(Enums.LoaiGioiTinh? gioiTinh)
        {
            if (gioiTinh == null || gioiTinh == Enums.LoaiGioiTinh.ChuaXacDinh)
            {
                return string.Empty;
            }

            return gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? "Nam" : "Nữ";
        }

        public string NamSinh { get; set; }

        public string DiaChi { get; set; }

        public string NguoiThucHien { get; set; }

        public DateTime NgayThucHien { get; set; }

        public string NgayThucHienDisplay => NgayThucHien.ApplyFormatDateTime();

        public string NguoiDuyetKq { get; set; }

        public DateTime? NgayDuyetKq { get; set; }

        public string NgayDuyetKqDisplay =>
            NgayDuyetKq != null ? NgayDuyetKq.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        //public bool? TrangThai { get; set; }

        //public string TrangThaiDisplay =>
        //    TrangThai != null ? TrangThai == true ? "Chờ duyệt (chạy lại)" : "Chờ duyệt" : "Đã duyệt";
        public bool? ChoKetQua { get; set; }
        public bool? ChoKetQuaChayLai { get; set; }
        public bool? ChoDuyetKetQua { get; set; }
        public bool? DaCoKetQua { get; set; }
        public int? TrangThai => ChoKetQuaChayLai == true ? 1 : (ChoKetQua != false ? 2 : (ChoDuyetKetQua == true ? 3 : 4));
        public string TrangThaiDisplay => TrangThai == 1 ? "Chờ KQ (chạy lại)" : (TrangThai == 2 ? "Đang thực hiện" : (TrangThai == 3 ? "Chờ duyệt" : "Đã duyệt"));
    }
}
