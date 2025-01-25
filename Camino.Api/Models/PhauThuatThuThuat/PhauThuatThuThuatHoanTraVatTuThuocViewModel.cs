using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class PhauThuatThuThuatHoanTraVatTuThuocViewModel : BaseViewModel
    {
        public PhauThuatThuThuatHoanTraVatTuThuocViewModel()
        {
            YeuCauDuocPhamVatTuBenhViens = new List<PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModel>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public long VatTuThuocBenhVienId { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        //public double? SoLuongTra { get; set; }
        //public double SoLuong { get; set; }
        //public double SoLuongDaTra { get; set; }
        public List<PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModel> YeuCauDuocPhamVatTuBenhViens { get; set; }
    }

    public class PhauThuatThuThuatHoanTraVatTuThuocChiTietViewModel : BaseViewModel
    {
        public EnumNhomGoiDichVu NhomYeuCauId { get; set; }
        public double? SoLuongTra { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaTra { get; set; }
    }

    public class PhauThuatThuThuatVatTuThuocViewModel
    {
        public long YeuCauId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? KhoId { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public long? VatTuThuocBenhVienId { get; set; }
        public double? SoLuong { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public EnumNhomGoiDichVu NhomYeuCauId { get; set; }
    }
}