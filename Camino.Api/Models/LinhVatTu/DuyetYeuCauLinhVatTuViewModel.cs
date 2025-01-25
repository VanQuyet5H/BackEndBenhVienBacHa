using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.LinhVatTu
{
    public class DuyetYeuCauLinhVatTuViewModel : BaseViewModel
    {
        public DuyetYeuCauLinhVatTuViewModel()
        {
            DuyetYeuCauLinhVatTuChiTiets = new List<DuyetYeuCauLinhVatTuChiTietViewModel>();
        }

        public long KhoXuatId { get; set; }

        public string TenKhoXuat { get; set; }

        public long KhoNhapId { get; set; }

        public string TenKhoNhap { get; set; }

        public long NhanVienDuyetId { get; set; }

        public string TenNhanVienDuyet { get; set; }

        public long NhanVienYeuCauId { get; set; }

        public string TenNhanVienYeuCau { get; set; }

        public DateTime? NgayYeuCau { get; set; }

        public DateTime? NgayDuyet { get; set; }

        public string GhiChu { get; set; }

        public long? NguoiXuatKhoId { get; set; }

        public string TenNguoiXuatKho { get; set; }

        public long? NguoiNhapKhoId { get; set; }

        public string TenNguoiNhapKho { get; set; }

        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }

        public string TenLoaiPhieuLinh => LoaiPhieuLinh.GetDescription();

        public bool? DuocDuyet { get; set; }
        public bool? DaGui { get; set; }

        public string TenTrangThai => GetTrangThai(DuocDuyet);

        public string LyDoKhongDuyet { get; set; }

        public List<DuyetYeuCauLinhVatTuChiTietViewModel> DuyetYeuCauLinhVatTuChiTiets { get; set; }

        private string GetTrangThai(bool? duocDuyet)
        {
            if (duocDuyet == null)
            {
                return "Đang chờ duyệt";
            }

            return duocDuyet == false ? "Từ chối duyệt" : "Đã duyệt";
        }
        public byte[] LastModified { get; set; }
        public bool? LaNguoiTaoPhieu { get; set; }

        public bool? LoaiDuocPhamHayVatTu { get; set; }

    }

    public class DuyetYeuCauLinhVatTuChiTietViewModel : BaseViewModel
    {
        public long? VatTuBenhVienId { get; set; }

        public bool? LaVatTuBHYT { get; set; }

        public string TenVatTu { get; set; }

        public double? SoLuong { get; set; }

        public double? SLTon { get; set; }

        public double? SoLuongCoTheXuat { get; set; }

        public string Nhom { get; set; }

        public long? DvtId { get; set; }

        public string DVT { get; set; }

        public string HangSanXuat { get; set; }

        public string NuocSanXuat { get; set; }
        public bool isTuChoi { get; set; }
        public int Index { get; set; }

        #region linhbu

        public long? YeuCauLinhVatTuId { get; set; }

        public bool? DuocDuyet { get; set; }

        public double? SlYeuCau { get; set; }

        public string HamLuong { get; set; }

        public string HoatChat { get; set; }

        public long? DuongDungId { get; set; }
   
        public string DuongDung { get; set; }

        public string NhaSx { get; set; }

        public double? SlCanBu { get; set; }
        #endregion
    }

    public class YeuCauVatTuBenhVienViewModel : BaseViewModel
    {
        public long? VatTuBenhVienId { get; set; }

        public bool? LaVatTuBhyt { get; set; }

        public string YeuCauVatTuBenhVien { get; set; }

        public long? KhoLinhTuId { get; set; }

        public double SoLuongCanBu { get; set; }

        public string TenVatTtu { get; set; }
    }
}
