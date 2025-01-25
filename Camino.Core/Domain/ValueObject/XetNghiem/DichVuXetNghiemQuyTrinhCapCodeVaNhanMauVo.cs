using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class DichVuXetNghiemQuyTrinhCapCodeVaNhanMauVo : GridItem
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public long? PhienXetNghiemChiTietId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public DateTime ThoiGianChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh.ApplyFormatDateTimeSACH();
        public string NguoiChiDinh { get; set; }
        public string BenhPham { get; set; }
        public Enums.EnumLoaiMauXetNghiem? LoaiMau { get; set; }
        public string TenLoaiMau => LoaiMau.GetDescription();
        public string TenNhom { get; set; }
        public string Barcode { get; set; }
        public int? BarcodeNumber { get; set; }
        public DateTime? NgayLayMau { get; set; }
        public string NgayLayMauDisplay => NgayLayMau?.ApplyFormatDateTimeSACH();
        public string NguoiLayMau { get; set; }
        public DateTime? NgayNhanMau { get; set; }
        public string NgayNhanMauDisplay => NgayNhanMau?.ApplyFormatDateTimeSACH();
        public string NguoiNhanMau { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTimeSACH();
        public string TenNguoiDuyet { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien?.ApplyFormatDateTimeSACH();
        public string TenNguoiThucHien { get; set; }
        public bool IsTaoPhienChiTiet { get; set; }
        public bool IsNhanVienKhoaXetNghiem { get; set; }
        public bool IsChayLaiKetQua { get; set; }

        public bool TatCaKetQuaChuaCoGiaTri { get; set; }
        public Enums.TrangThaiLayMauXetNghiemNew TrangThai
        {
            get
            {
                if (NgayLayMau == null)
                {
                    //return Enums.TrangThaiLayMauXetNghiem.ChoLayMau;
                    return Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode;

                }
                else if (NgayLayMau != null && NgayNhanMau == null)// chờ nhận mẫu/ đã cấp barcode
                {
                    //return Enums.TrangThaiLayMauXetNghiem.ChoGuiMau;
                    return Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau;
                }
                else if (TatCaKetQuaChuaCoGiaTri)// chờ kết quả
                {
                    //return Enums.TrangThaiLayMauXetNghiem.ChoKetQua;
                    return Enums.TrangThaiLayMauXetNghiemNew.ChoKetQua;
                }
                else if (NgayDuyet != null)// đã duyệt
                {
                    //return Enums.TrangThaiLayMauXetNghiem.ChoKetQua;
                    return Enums.TrangThaiLayMauXetNghiemNew.DaDuyet;
                }
                // đã có kết quả
                else
                {
                    //return Enums.TrangThaiLayMauXetNghiem.DaCoKetQua;
                    return Enums.TrangThaiLayMauXetNghiemNew.DaCoKetQua;

                }
            }
        }
        public string TenTrangThai => TrangThai.GetDescription();
        public bool IsShowXacNhanNhanMau => NgayLayMau != null && NgayNhanMau == null && IsNhanVienKhoaXetNghiem;
        //public bool IsShowHuyMau => TrangThai != Enums.TrangThaiLayMauXetNghiem.DaCoKetQua && !IsChayLaiKetQua;
        public bool IsShowHuyMau => TrangThai != Enums.TrangThaiLayMauXetNghiemNew.DaCoKetQua && !IsChayLaiKetQua;
        //public bool IsShowEditNgayNhanMau => TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua
        //                                     && IsNhanVienKhoaXetNghiem;
        public bool IsShowEditNgayNhanMau => TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoKetQua
                                             && IsNhanVienKhoaXetNghiem;

        //BVHD-3372
        public XacNhanCapCodeTrangThaiTimKiemNangCapVo PhanLoaiDichVu { get; set; }
        public bool IsActive
        {
            get
            {
                // màn hình chờ cấp code, cấp code
                if (PhanLoaiDichVu.ChuaCapCode)
                {
                    //return TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoLayMau;
                    return TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoCapCode;
                }
                else if (PhanLoaiDichVu.DaCapCode)
                {
                    //return TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau;
                    return TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau;
                }

                //màn hình nhận mẫu
                else if (PhanLoaiDichVu.ChuaNhanMau)
                {
                    //return TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau;
                    return TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoNhanMau;
                }
                else if (PhanLoaiDichVu.DaNhanMau)
                {
                    //return TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua || TrangThai == Enums.TrangThaiLayMauXetNghiem.DaCoKetQua;
                    return TrangThai == Enums.TrangThaiLayMauXetNghiemNew.ChoKetQua || TrangThai == Enums.TrangThaiLayMauXetNghiemNew.DaCoKetQua;
                }

                return false;
            }
        }

        // BVHD-3499
        public int SoThuTuXetNghiem { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public DateTime? CreatedOn { get; set; }

        //Cập nhật 29/03/2022
        public long? PhienChiTietCuoiCungHienTaiId { get; set; }
    }

    public class ThongTinSoLuongInThemTheoTaiKhoanVo
    {
        public long UserId { get; set; }
        public int SoLuong { get; set; }
    }
}
