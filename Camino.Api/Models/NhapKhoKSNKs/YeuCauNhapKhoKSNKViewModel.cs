using Camino.Core.Domain;
using System;
using System.Collections.Generic;

namespace Camino.Api.Models.NhapKhoKSNKs
{
    public class YeuCauNhapKhoKSNKViewModel : BaseViewModel
    {
        public YeuCauNhapKhoKSNKViewModel()
        {
            NhapKhoVatTuChiTiets = new List<YeuCauNhapKhoVatTuChiTietViewModel>();
            OldNhapKhoVatTuChiTiets = new List<YeuCauNhapKhoVatTuChiTietViewModel>();
        }

        public long? NhanVienDuyetId { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string KyHieuHoaDon { get; set; }
        public string SoChungTu { get; set; }
        public string TenNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }
        public long? NguoiNhapId { get; set; }
        public Enums.LoaiNguoiGiaoNhan? LoaiNguoiGiao { get; set; }
        public DateTime? NgayNhap { get; set; }

        public string LyDoKhongDuyet { get; set; }

        public bool? DuocKeToanDuyet { get; set; }
        public DateTime? NgayHoaDon { get; set; }

        public List<YeuCauNhapKhoVatTuChiTietViewModel> NhapKhoVatTuChiTiets { get; set; }
        public List<YeuCauNhapKhoVatTuChiTietViewModel> OldNhapKhoVatTuChiTiets { get; set; }


        //Thêm mặc định kho vật tư y tế
        public long? KhoVatTuYTeId { get; set; }
        public string KhoVatTuYTeDisplayName { get; set; }
    }

    public class YeuCauNhapKhoVatTuChiTietViewModel : BaseViewModel
    {
        public int? TiLeBHYTThanhToan { get; set; }

        public long YeuCauNhapKhoVatTuId { get; set; }
        public bool DaCapNhat { get; set; }
        public bool DaCongDon { get; set; }
        public long? HopDongThauVatTuId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public long? VatTuBenhVienPhanNhomId { get; set; }
        public string Solo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string MaVach { get; set; }
        public int? SoLuongNhap { get; set; }
        public double? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public long? KhoViTriId { get; set; }
        public long? NhaThauId { get; set; }
        public string NhaThauDisplay { get; set; }
        public int LoaiNhap { get; set; }
        //for grid
        public string HopDongThauDisplay { get; set; }
        public string VatTuDisplay { get; set; }
        public string LoaiDisplay { get; set; }
        public string DatChatLuongDisplay { get; set; }
        public string HanSuDungDisplay { get; set; }
        public string SoLuongNhapDisplay { get; set; }
        public string DonGiaNhapDisplay { get; set; }
        //public string ViTriDisplay { get; set; }
        public string NhomDisplay { get; set; }
        public string LoaiSuDungDisplay { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }

        public string DVT { get; set; }
        public string MaRef { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long? NguoiNhapSauKhiDuyetId { get; set; }
        public string TenKhoNhapSauKhiDuyet { get; set; }
        public string TenNguoiNhapSauKhiDuyet { get; set; }
        public decimal ThanhTienTruocVat { get; set; }
        public decimal ThanhTienSauVat { get; set; }
        public decimal? ThueVatLamTron { get; set; }
        public string GhiChu { get; set; }

    }
}
