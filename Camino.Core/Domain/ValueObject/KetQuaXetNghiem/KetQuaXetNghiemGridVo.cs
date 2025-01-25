using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Helpers.Constants;

namespace Camino.Core.Domain.ValueObject.KetQuaXetNghiem
{
    public class KetQuaXetNghiemSearch
    {
        public bool ChoKetQua { get; set; }
        public bool ChoDuyetKetQua { get; set; }
        public bool DaCoKetQua { get; set; }
        public string SearchString { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class KetQuaXetNghiemGridVo : GridItem
    {
        public string BarCode { get; set; }
        public string BarCodeNumber { get; set; }
        public string MaSoBHYT { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string NguoiThucHien { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public DateTime? ThoiDiemBatDau { get; set; }
        public string NgayThucHienDisplay => NgayThucHien?.ApplyFormatDateTime();
        public string NguoiDuyetKQ { get; set; }
        public DateTime? NgayDuyetKQ { get; set; }
        public string NgayDuyetKQDisplay => NgayDuyetKQ?.ApplyFormatDateTime();
        public bool? ChoKetQua { get; set; }
        public bool? ChoKetQuaChayLai { get; set; }
        public bool? ChoDuyetKetQua { get; set; }
        public bool? DaCoKetQua { get; set; }
        public int? TrangThai => ChoKetQuaChayLai == true ? 1 : (ChoKetQua != false ? 2 : (ChoDuyetKetQua == true ? 3 : 4));
        public string SearchString { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TrangThaiDisplay => TrangThai == 1 ? "Chờ KQ (chạy lại)" : (TrangThai == 2 ? "Đang thực hiện" : (TrangThai == 3 ? "Chờ duyệt" : "Đã duyệt"));
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class KetQuaXetNghiemChildGridVo : GridItem
    {
        public string TenNhomDichVuBenhVien { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public DateTime ThoiGianChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay { get { return ThoiGianChiDinh.ApplyFormatDateTime(); } }
        public string NguoiChiDinh { get; set; }
        public string BenhPham { get; set; }
        public string LoaiMau { get; set; }

        //
        public bool? YeuCauChayLai { get; set; }
        public bool? DaDuyet { get; set; }
        public string NguoiYeuCau { get; set; }
        //public DateTime? NgayYeuCau { get; set; }
        //public string NgayYeuCauDisplay { get { return NgayYeuCau != null ? (NgayYeuCau ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
        public string NgayYeuCauDisplay { get; set; }
        public string LyDoYeuCau { get; set; }
        public string NguoiDuyet { get; set; }
        //public DateTime? NgayDuyet { get; set; }
        //public string NgayDuyetDisplay { get { return NgayDuyet != null ? (NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
        public string NgayDuyetDisplay { get; set; }
        public List<string> DanhSachLoaiMau { get; set; }
        public List<string> DanhSachLoaiMauTongCong { get; set; }
        public List<string> DanhSachLoaiMauKhongDat { get; set; }
    }
}
