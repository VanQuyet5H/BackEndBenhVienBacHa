using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems
{
    public class DuyetKetQuaXetNghiemPhieuInResultVo
    {
        public string MaBHYT { get; set; }
        public string DanhSach { get; set; }

        public string LogoUrl { get; set; }

        public string SoPhieu { get; set; }

        public string SoVaoVien { get; set; }

        public string MaYTe { get; set; }

        public string BarCodeImgBase64 { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }
        public string NamSinhString { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string GioiTinhDisplay => ShowGioiTinhTxt(GioiTinh);

        private string ShowGioiTinhTxt(Enums.LoaiGioiTinh? gioiTinh)
        {
            if (gioiTinh == null)
            {
                return string.Empty;
            }

            return gioiTinh.GetDescription();
        }

        public string DiaChi { get; set; }

        public string LoaiMau { get; set; }

        public string DoiTuong { get; set; }

        public string BsChiDinh { get; set; }

        public string ChanDoan { get; set; }

        public string GhiChu { get; set; }

        public string KhoaPhong { get; set; }

        public string TgLayMau { get; set; }

        public string NguoiLayMau { get; set; }

        public string TgNhanMau { get; set; }

        public string NguoiNhanMau { get; set; }

        public string NguoiThucHien { get; set; }

        public string KetQuaXetNghiem { get; set; }

        public string Gio { get; set; }

        public string Ngay { get; set; }

        public string Thang { get; set; }

        public string Nam { get; set; }

        public List<ListDataChildVo> ChiTietKetQuaXetNghiems { get; set; }

        public string Barcode { get; set; }
        public string DienGiai { get; set; }
        public string NgayThangNamFooter { get; set; }
        public string STT { get; set; }
        public string LogoBV1 { get; set; }
        public string LogoBV2 { get; set; }
        public string GhiChuDV { get; set; }
    }

    public class ListDataChildVo : GridItem
    {
        public int LoaiKetQuaTuMay { get; set; } = 1;

        public string Ten { get; set; }

        public long YeuCauDichVuKyThuatId { get; set; }

        public string GiaTriCu { get; set; }

        public string GiaTriTuMay { get; set; }

        public string GiaTriNhapTay { get; set; }

        public string GiaTriDuyet { get; set; }

        public bool? ToDamGiaTri { get; set; }

        public string Csbt { get; set; }

        public string DonVi { get; set; }

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

        public bool? YeuCauChayLai { get; set; }

        public bool? DaDuyet { get; set; }

        public string NguoiYeuCau { get; set; }

        public string NgayYeuCauDisplay { get; set; }

        public string LyDoYeuCau { get; set; }

        public string NguoiDuyetChayLai { get; set; }

        public string NgayDuyetDisplay { get; set; }

        public string Nhom { get; set; }
        public string  STT { get; set; }
        
    }

    public class NhomDichVuXetNghiemDuyetVo
    {
        public bool IsCheck { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string TenNhomDichVu { get; set; }
    }

  
}
