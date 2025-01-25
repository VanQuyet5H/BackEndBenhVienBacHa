using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoKhacPhieuSangLocDinhDuongViewModel : BaseViewModel
    {
        public HoSoKhacPhieuSangLocDinhDuongViewModel()
        {
            FilesChuKy = new List<FileChuKyPhieuSangLocDinhDuongViewModel>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTimeSACH();
        public LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHienDisplay { get; set; }
        public long? NoiThucHienId { get; set; }
        public string NoiThucHienDisplay { get; set; }
        public string ChanDoan { get; set; }
        public List<FileChuKyPhieuSangLocDinhDuongViewModel> FilesChuKy { get; set; }
    }

    public class FileChuKyPhieuSangLocDinhDuongViewModel : BaseViewModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class DSPhieuSangLocDinhDuong
    {
        public DateTime? NgayDanhGia { get; set; }
        public string NgayDanhGiaString { get; set; }
        public string NgayDanhGiaDisplay { get; set; }
        public string NguyCoDinhDuong { get; set; }
        public long? CheDoAnUong { get; set; }
        public string CheDoAnUongDisplay { get; set; }
        public long? NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool? DungChoPhuNuMangThai { get; set; }
    }
    public class InFoSaveUpDatePhieuSangLocDinhDuong
    {
        public bool SaveOrUpdate { get; set; }
        public long NoiTruHoSoKhacId { get; set; }
    }
    public class ReMoveModel
    {
        public bool DungChoPhuNuMangThai { get; set; }
        public long YeuCauTiepNhanId { get; set; }
    }
}
