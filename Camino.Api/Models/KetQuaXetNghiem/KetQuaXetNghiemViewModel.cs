using Camino.Core.Domain;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KetQuaXetNghiem
{
    public class KetQuaXetNghiemViewKetQuaViewModel : BaseViewModel
    {
        public KetQuaXetNghiemViewKetQuaViewModel()
        {
            dataChild = new List<ListDataChild>();
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


        public long? NhanVienThucHienId { get; set; }
        public string ChanDoanDuoi { get; set; }
        public string GhiChu { get; set; }
        //
        public List<string> DanhSachLoaiMau { get; set; }
        //
        public bool? YeuCauChayLai { get; set; }
        public bool? DaDuyet { get; set; }
        //public string NguoiYeuCau { get; set; }

        public string NgayYeuCauDisplay { get; set; }
        public string LyDoYeuCau { get; set; }
        //public string NguoiDuyet { get; set; }

        public string NgayDuyetDisplay { get; set; }

        //
        public List<ListDataChild> dataChild { get; set; }
        //public bool? OnlySave { get; set; }

        //BVHD-3364
        public string TenCongTy { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    //public class KetQuaXetNghiemChiTietViewModel : BaseViewModel
    //{
    //    public KetQuaXetNghiemChiTietViewModel()
    //    {
    //        datas = new List<ListDataChild>();
    //    }
    //    public List<ListDataChild> datas { get; set; }
    //    public string TenNhomDichVuBenhVien { get; set; }
    //    public long NhomDichVuBenhVienId { get; set; }
    //    //
    //    public bool? YeuCauChayLai { get; set; }
    //    public bool? DaDuyet { get; set; }
    //    public string NguoiYeuCau { get; set; }
    //    //public DateTime? NgayYeuCau { get; set; }
    //    //public string NgayYeuCauDisplay { get { return NgayYeuCau != null ? (NgayYeuCau ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
    //    public string NgayYeuCauDisplay { get; set; }
    //    public string LyDoYeuCau { get; set; }
    //    public string NguoiDuyet { get; set; }
    //    //public DateTime? NgayDuyet { get; set; }
    //    //public string NgayDuyetDisplay { get { return NgayDuyet != null ? (NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
    //    public string NgayDuyetDisplay { get; set; }
    //    public List<string> DanhSachLoaiMauDaCoKetQua { get; set; }
    //    public List<string> DanhSachLoaiMau { get; set; }
    //}

    public class ListDataChild : BaseViewModel
    {
        //public ListDataChild()
        //{
        //    Items = new List<ListDataChild>();
        //}
        //1 bình thường, 2 bất thường, 3 nguy hiểm
        public bool? CheckBox { get; set; }
        public bool? CheckBoxParent { get; set; }
        public bool? DisabledCheckBoxParent => DaGoiDuyet == true;

        public bool? DaGoiDuyet { get; set; }

        public int LoaiKetQuaTuMay { get; set; } = 1;
        public string Ten { get; set; }
        public long yeuCauDichVuKyThuatId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string GiaTriCu { get; set; }
        public string GiaTriTuMay { get; set; }
        public string GiaTriNhapTay { get; set; }
        public string GiaTriDuyet { get; set; }
        public bool? ToDamGiaTri { get; set; }
        public string CSBT { get; set; }
        public string GiaTriMin { get; set; }
        public string GiaTriMax { get; set; }
        public string DonVi { get; set; }
        public DateTime? ThoiDiemGuiYeuCau { get; set; }
        public string ThoiDiemGuiYeuCauDisplay { get { return ThoiDiemGuiYeuCau != null ? (ThoiDiemGuiYeuCau ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
        public DateTime? ThoiDiemNhanKetQua { get; set; }
        public string ThoiDiemNhanKetQuaDisplay { get { return ThoiDiemNhanKetQua != null ? (ThoiDiemNhanKetQua ?? DateTime.Now).ApplyFormatDateTime() : ""; } }

        public long? MayXetNghiemId { get; set; }
        public string TenMayXetNghiem { get; set; }

        public DateTime? ThoiDiemDuyetKetQua { get; set; }
        public string ThoiDiemDuyetKetQuaDisplay { get { return ThoiDiemDuyetKetQua != null ? (ThoiDiemDuyetKetQua ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
        public string NguoiDuyet { get; set; }


        //public List<ListDataChild> Items { get; set; }

        //
        public string LoaiMau { get; set; }
        public long DichVuXetNghiemId { get; set; }

        //update
        public List<long> IdChilds { get; set; } = new List<long>();
        public int Level { get; set; }
        //
        public List<string> DanhSachLoaiMau { get; set; }
        public List<string> DanhSachLoaiMauDaCoKetQua { get; set; }
        public List<string> DanhSachLoaiMauKhongDat { get; set; }
        //
        public bool? YeuCauChayLai { get; set; }
        public bool? DaDuyet { get; set; }
        public string NguoiYeuCau { get; set; }

        public string NgayYeuCauDisplay { get; set; }
        public string LyDoYeuCau { get; set; }
        public string NguoiDuyetChayLai { get; set; }

        public string NgayDuyetDisplay { get; set; }

        public string Nhom { get; set; }
        //
        public bool DaDuyetChiTiet { get; set; }
        public long? DichVuXetNghiemChaId { get; set; }
        public string NguoiThucHien { get; set; }
    }
}
