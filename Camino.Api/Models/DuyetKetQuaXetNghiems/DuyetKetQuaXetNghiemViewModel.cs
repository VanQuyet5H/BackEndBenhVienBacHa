using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.DuyetKetQuaXetNghiems
{
    public class DuyetKetQuaXetNghiemViewModel : BaseViewModel
    {
        public DuyetKetQuaXetNghiemViewModel()
        {
            ChiTietKetQuaXetNghiems = new List<DuyetKqXetNghiemChiTietViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }

        #region Thông tin hành chính
        public string BarCodeID { get; set; }
        public string MaBN { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NamSinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay { get { return GioiTinh.GetDescription(); } }
        public string SoDienThoai { get; set; }
        public string MaSoBHYT { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public int? BHYTMucHuong { get; set; }
        public bool? CoBHYT { get; set; }
        public string DoiTuong
        {
            get
            {
                var doiTuong = string.Empty;
                if (LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                {
                    doiTuong = "Khám sức khỏe";
                }
                else
                {
                    if (BHYTMucHuong != null && BHYTMucHuong != 0 && CoBHYT == true)
                    {
                        doiTuong = $"BHYT ({BHYTMucHuong}%)";
                    }
                    else
                    {
                        doiTuong = "Viện phí";
                    }
                }
                return doiTuong;
            }
        }
        public string DiaChi { get; set; }
        public string ChanDoan { get; set; }

        public string KhoaChiDinh { get; set; }
        public string Phong { get; set; }
        #endregion

        public string NguoiThucHien { get; set; }
        public long? NguoiThucHienId { get; set; }
        public string GhiChu { get; set; }
        public bool? TrangThai { get; set; }
        public string TrangThaiDisplay =>
            TrangThai != null ? TrangThai == true ? "Đang thực hiện" : "Chờ duyệt" : "Đã duyệt";

        public List<DuyetKqXetNghiemChiTietViewModel> ChiTietKetQuaXetNghiems { get; set; }

        //BVHD-3364
        public string TenCongTy { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class DuyetKqXetNghiemChiTietViewModel : BaseViewModel
    {
        public int LoaiKetQuaTuMay { get; set; } = 1;
        public int LoaiKetQuaNhapTay { get; set; } = 1;

        public long NhomDichVuBenhVienId { get; set; }

        public string Ten { get; set; }

        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }   

        public string GiaTriCu { get; set; }

        public string GiaTriTuMay { get; set; }

        public string GiaTriNhapTay { get; set; }

        public string GiaTriDuyet { get; set; }

        public bool? ToDamGiaTri { get; set; }

        public string Csbt { get; set; }
        public string GiaTriMin { get; set; }
        public string GiaTriMax { get; set; }

        public bool? DaGoiDuyet { get; set; }

        public string DonVi { get; set; }

        public bool Duyet { get; set; }
        public bool? IsParent { get; set; }
        public string LoaiKitThu { get; set; }
        public long? LoaiKitThuId { get; set; }
        public bool LaDichVuSarCovid2 { get; set; }

        public DateTime? ThoiDiemGuiYeuCau { get; set; }

        public string ThoiDiemGuiYeuCauDisplay => ThoiDiemGuiYeuCau != null ? (ThoiDiemGuiYeuCau ?? DateTime.Now).ApplyFormatDateTime() : "";

        public DateTime? ThoiDiemNhanKetQua { get; set; }

        public string ThoiDiemNhanKetQuaDisplay => ThoiDiemNhanKetQua != null ? (ThoiDiemNhanKetQua ?? DateTime.Now).ApplyFormatDateTime() : "";

        public long? MayXetNghiemId { get; set; }

        public string TenMayXetNghiem { get; set; }

        public DateTime? ThoiDiemDuyetKetQua { get; set; }

        public string ThoiDiemDuyetKetQuaDisplay => ThoiDiemDuyetKetQua != null ? (ThoiDiemDuyetKetQua ?? DateTime.Now).ApplyFormatDateTime() : "";

        public string NguoiDuyet { get; set; }

        public string LoaiMau { get; set; }

        public long DichVuXetNghiemId { get; set; }

        public List<long> IdChilds { get; set; } = new List<long>();

        public int Level { get; set; }

        public List<string> DanhSachLoaiMau { get; set; }

        public List<string> DanhSachLoaiMauDaCoKetQua { get; set; }

        public List<string> DanhSachLoaiMauKhongDat { get; set; }

        public bool? YeuCauChayLai { get; set; }

        public bool? DaDuyet { get; set; }

        public string NguoiYeuCau { get; set; }

        public string NgayYeuCauDisplay { get; set; }

        public string LyDoYeuCau { get; set; }

        public string NguoiDuyetChayLai { get; set; }

        public string NgayDuyetDisplay { get; set; }

        public string Nhom { get; set; }

        public long NhomId { get; set; }
        public long? DichVuXetNghiemChaId { get; set; }
        public bool? CheckBox { get; set; }

        public string NguoiThucHien { get; set; }

    }
}
