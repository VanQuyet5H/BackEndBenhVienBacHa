using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhoVatTus
{
    public class NhapKhoVatTuGridVo : GridItem
    {
        public string SoChungTu { get; set; }
        public string SoPhieu { get; set; }

        public long NguoiNhapId { get; set; }
        public string TenNguoiNhap { get; set; }

        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public string LoaiNguoiGiaoDisplay
        {
            get { return LoaiNguoiGiao.GetDescription(); }
        }

        public long? NguoiGiaoId { get; set; }
        public string TenNguoiGiao { get; set; }

        public DateTime? NgayNhap { get; set; }
        public string NgayNhapDisplay
        {
            get { return NgayNhap != null ? (NgayNhap ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }

        public bool? DuocKeToanDuyet { get; set; }
        //public string TinhTrangDisplay
        //{
        //    get { return DuocKeToanDuyet != null ? (DuocKeToanDuyet == true ? "Đã duyệt" : "Từ chối duyệt") : "Đang chờ duyệt"; }
        //}
        public string TinhTrangDisplay => TinhTrang == 0 ? "Đang chờ duyệt" : (TinhTrang == 1 ? "Đã duyệt" : "Từ chối duyệt");
        public int? TinhTrang
        {
            get { return DuocKeToanDuyet != null ? (DuocKeToanDuyet == true ? 1 : 2) : 0; }
        }

        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay
        {
            get { return NgayDuyet != null ? (NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay
        {
            get { return NgayHoaDon != null ? (NgayHoaDon ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }
        public string TenKho { get; set; }

        //BVHD-3926
        public string TenNhaCungCap { get; set; }
        public List<DataYeuCauNhapKhoVatTuChiTiet> DataYeuCauNhapKhoVatTuChiTiets { get; set; } = new List<DataYeuCauNhapKhoVatTuChiTiet>();
    }

    public class DataYeuCauNhapKhoVatTuChiTiet
    {
        public long Id { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long HopDongThauVatTuId { get; set; }
    }

    public class NhapKhoVatTuChiTietGripVo : GridItem
    {
        public int? TiLeBHYTThanhToan { get; set; }
        public string TenVatTu { get; set; }
        public string NhaThauDisplay { get; set; }
        public string TenHDThau { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Loai { get { return LaVatTuBHYT ? "BHYT" : "Không BHYT"; } }
        public string LoaiSuDung { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay
        {
            get { return HanSuDung.ApplyFormatDate(); }
        }
        public string MaVach { get; set; }
        public double SL { get; set; }
        public decimal GiaNhap { get; set; }
        public string SLDisplay { get { return SL.ApplyNumber(); } }
        public string GiaNhapDisplay { get { return GiaNhap.ApplyVietnameseFloatNumber(); } }
        public int VAT { get; set; }
        public string ViTri { get; set; }
        public string MaRef { get; set; }
    }
}
