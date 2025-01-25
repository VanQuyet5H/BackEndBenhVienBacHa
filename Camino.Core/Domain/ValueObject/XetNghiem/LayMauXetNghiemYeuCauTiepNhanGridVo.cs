using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class LayMauXetNghiemYeuCauTiepNhanGridVo : GridItem
    {
        public LayMauXetNghiemYeuCauTiepNhanGridVo()
        {
            NhomCanLayMauXetNghiems = new List<NhomCanLayMauXetNghiemGridVo>();
        }
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public int SoLuongChoLay { get; set; }
        public int SoLuongChoGui { get; set; }
        public bool FlagChoLay => SoLuongChoLay > 0;
        public bool FlagChoGui => SoLuongChoGui > 0;
        public bool FlagChoKetQua { get; set; }
        public bool FlagDaCoKetQua { get; set; }
        public bool CoDuKetQua { get; set; }
        public bool FlagCoDichVuDaCoKetQua { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public bool BenhNhanDaNhanKetQua { get; set; }
        public DateTime? NgayLayMau { get; set; }
        public int STT { get; set; }
        public string BarcodeDisplay { get; set; }

        public List<NhomCanLayMauXetNghiemGridVo> NhomCanLayMauXetNghiems { get; set; }
    }

    public class NhomCanLayMauXetNghiemGridVo : GridItem
    {
        public NhomCanLayMauXetNghiemGridVo()
        {
            LoaiMaus = new List<LoaiMauVo>();
            DichVuCanLayMauXetNghiems = new List<DichVuCanLayMauXetNghiemGridVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public long BenhNhanId { get; set; }
        public int? NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string TenNhom { get; set; }
        public string Barcode { get; set; }
        public int? BarcodeNumber { get; set; }

        public Enums.TrangThaiLayMauXetNghiem TrangThai
        {
            get
            {
                if (SoLuongMauDaTao < LoaiMaus.Count || (NgayLayMau == null && string.IsNullOrEmpty(Barcode)) || LoaiMaus.Any(x => x.DatChatLuong == false))
                {
                    return Enums.TrangThaiLayMauXetNghiem.ChoLayMau;
                }
                else if (SoLuongMauDaTao == LoaiMaus.Count && NgayLayMau != null)

                {
                    //chờ gửi mẫu
                    if (NgayGui == null)
                        return Enums.TrangThaiLayMauXetNghiem.ChoGuiMau;
                    else
                    {
                        // chờ kết quả
                        if (NgayDuyet == null)
                        {
                            return Enums.TrangThaiLayMauXetNghiem.ChoKetQua;
                        }
                        // đã có kết quả
                        else
                        {
                            return Enums.TrangThaiLayMauXetNghiem.DaCoKetQua;
                        }
                    }
                }
                else
                {
                    return Enums.TrangThaiLayMauXetNghiem.ChoLayMau;
                }


                //// chờ kết quả
                //else if (SoLuongMauDaTao == LoaiMaus.Count && (NgayNhan != null || NgayGui != null) && NgayDuyet == null)
                //{
                //    return Enums.TrangThaiLayMauXetNghiem.ChoKetQua;
                //}
                //// đã có kết quả
                //else if (SoLuongMauDaTao == LoaiMaus.Count && NgayDuyet != null)
                //{
                //    return Enums.TrangThaiLayMauXetNghiem.DaCoKetQua;
                //}
                //else
                //{
                //    return Enums.TrangThaiLayMauXetNghiem.ChoLayMau;
                //}

                //// đã có kết quả
                //else if (NgayDuyet != null)
                //{
                //    return Enums.TrangThaiLayMauXetNghiem.DaCoKetQua;
                //}
                //// chờ kết quả
                //else if (NgayDuyet == null && NgayNhan == null && NgayGui != null)
                //{
                //    return Enums.TrangThaiLayMauXetNghiem.ChoKetQua;
                //}
                ////chờ gửi mẫu
                //else if (NgayDuyet == null && NgayNhan == null && NgayGui == null && (NgayLayMau != null || !string.IsNullOrEmpty(Barcode)))
                //{
                //    return Enums.TrangThaiLayMauXetNghiem.ChoGuiMau;
                //}
                //else
                //{
                //    return Enums.TrangThaiLayMauXetNghiem.ChoLayMau;
                //}
            }
        }

        public string TenTrangThai => TrangThai.GetDescription();
        public LoaiMauVo LoaiMau { get; set; }
        public List<LoaiMauVo> LoaiMaus { get; set; }

        public string LyDoTuChoi
        {
            get
            {
                var lyDo = string.Empty;
                var loaiMauBiTuChoi = LoaiMaus.FirstOrDefault(x => x.DatChatLuong == false);
                if (loaiMauBiTuChoi != null)
                {
                    lyDo = "Mẫu bị hủy bởi " + loaiMauBiTuChoi.TenNhanVienXetKhongDat + 
                           " vào ngày: " + loaiMauBiTuChoi.ThoiDiemXetKhongDatDisplay + 
                           " Lý do: " + loaiMauBiTuChoi.LyDoKhongDat;
                }

                return lyDo;
            }
        }
        public bool IsTuChoiMau => LoaiMaus.Any(x => x.DatChatLuong != null && !x.DatChatLuong.Value);

        public int SoLuongMauDaTao { get; set; }
        public string SoPhieu { get; set; }
        public long? PhieuGuiMauXetNghiemId { get; set; }
        public DateTime? NgayLayMau { get; set; }
        public string NgayLayMauDisplay => NgayLayMau?.ApplyFormatDateTimeSACH();
        public long? NguoiLayMauId { get; set; }
        public string TenNguoiLayMau { get; set; }
        public DateTime? NgayGui { get; set; }
        public string NgayGuiDisplay => NgayGui?.ApplyFormatDateTimeSACH();
        public string TenNGuoiGui { get; set; }
        public DateTime? NgayNhan { get; set; }
        public string NgayNhanDisplay => NgayNhan?.ApplyFormatDateTimeSACH();
        public long? NguoiNhanId { get; set; }
        public string TenNguoiNhan { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien?.ApplyFormatDateTimeSACH();
        public long? NguoiThucHienId { get; set; }
        public string TenNguoiThucHien { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTimeSACH();
        public long? NguoiDuyetId { get; set; }
        public string TenNguoiDuyet { get; set; }

        public List<DichVuCanLayMauXetNghiemGridVo> DichVuCanLayMauXetNghiems { get; set; }

        public bool IsShowXacNhanLayMau => NgayLayMau == null;
        public bool IsShowCapNhatLayMau => NgayLayMau != null && LoaiMaus.Any(x => x.DatChatLuong == false);
        public bool IsShowHuyMau => TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoGuiMau || TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua;
        public DateTime? ThoiDiemChiDinhHoacNgayDieuTri { get; set; }
        public string ThoiDiemChiDinhHoacNgayDieuTriDisplay => ThoiDiemChiDinhHoacNgayDieuTri?.ApplyFormatDateTimeSACH();
    }

    public class LoaiMauVo
    {
        public Enums.EnumLoaiMauXetNghiem? LoaiMau { get; set; }
        public string TenLoaiMau => LoaiMau.GetDescription();
        public bool? DatChatLuong { get; set; }
        public string TenNhanVienXetKhongDat { get; set; }
        public string LyDoKhongDat { get; set; }
        public DateTime? ThoiDiemXetKhongDat { get; set; }

        public string ThoiDiemXetKhongDatDisplay =>
            ThoiDiemXetKhongDat != null ? ThoiDiemXetKhongDat.Value.ApplyFormatDateTimeSACH() : null;
        public int TrangThai => DatChatLuong == null ? 1 : (DatChatLuong == true ? 2 : 3);
    }

    public class DichVuCanLayMauXetNghiemGridVo : GridItem
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

        public Enums.TrangThaiLayMauXetNghiem TrangThai
        {
            get
            {
                if (NgayLayMau == null)
                {
                    return Enums.TrangThaiLayMauXetNghiem.ChoLayMau;
                }
                else if (NgayLayMau != null && NgayNhanMau == null)// chờ nhận mẫu/ đã cấp barcode
                {
                    return Enums.TrangThaiLayMauXetNghiem.ChoGuiMau;
                }
                else if (NgayDuyet == null)// chờ kết quả
                {
                    return Enums.TrangThaiLayMauXetNghiem.ChoKetQua;
                }
                // đã có kết quả
                else
                {
                    return Enums.TrangThaiLayMauXetNghiem.DaCoKetQua;
                }
            }
        }
        public string TenTrangThai => TrangThai.GetDescription();
        public bool IsShowXacNhanNhanMau => NgayLayMau != null && NgayNhanMau == null && IsNhanVienKhoaXetNghiem;
        public bool IsShowHuyMau => TrangThai != Enums.TrangThaiLayMauXetNghiem.DaCoKetQua && !IsChayLaiKetQua;
        public bool IsShowEditNgayNhanMau => TrangThai == Enums.TrangThaiLayMauXetNghiem.ChoKetQua
                                             && IsNhanVienKhoaXetNghiem;

        public bool HideCheckbox => !IsShowXacNhanNhanMau && !IsShowHuyMau;
    }

    public class ThongTinYeuCauTiepNhanLayMauVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public int Tuoi => NamSinh == null ? 0 : DateTime.Now.Year - NamSinh.Value;
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenGioiTinh => GioiTinh.GetDescription();
        public string Tuyen { get; set; }
        public int? MucHuong { get; set; }
        public string DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string SoDienThoai { get; set; }
        public string SoTheBHYT { get; set; }
        public bool IsCoDuKetQua { get; set; }
        public bool IsTraKetQua { get; set; }
        public bool IsAutoBarcode { get; set; }
        public string TraKetQuaChoBenhNhan => IsTraKetQua ? "Đã trả" : "Chưa trả";
        public bool IsCoPhienChiTietCoKetQua { get; set; }
        public bool IsNhanVienKhoaXetNghiem { get; set; }

        //BVHD-3364
        public string TenCongTy { get; set; }
    }

    public class MauXetNghiemCanLayLaiVo
    {
        public Enums.EnumLoaiMauXetNghiem LoaiMauXetNghiem { get; set; }
        public bool DatChatLuong { get; set; }
        public int BarcodeNumber { get; set; }
        public string BarcodeId { get; set; }
    }


    #region Vo Lịch sử từ chối
    public class LichSuTuChoiMauVo
    {
        public DateTime? ThoiGianThucHien { get; set; }

        public string ThoiGianThucHienDisplay =>
            ThoiGianThucHien != null ? ThoiGianThucHien.Value.ApplyFormatDateTimeSACH() : null;
        public string NguoiTuChoi { get; set; }
        public string LyDoTuChoi { get; set; }
        public LichSuTuChoiMauItemDataSource LichSuTuChoiMauGridVo { get; set; }
        public LichSuTuChoiMauItemVo LichSuTuChoiMauItem { get; set; }
    }

    public class LichSuTuChoiMauItemDataSource
    {
        public LichSuTuChoiMauItemDataSource()
        {
            data = new List<LichSuTuChoiMauItemVo>();
        }
        public List<LichSuTuChoiMauItemVo> data { get; set; }
        public int total => data.Count;
    }

    public class LichSuTuChoiMauItemVo : GridItem
    {
        public string TenNhom { get; set; }
        public string TenMau { get; set; }
        public string Barcode { get; set; }
        public DateTime? NgayLayMau { get; set; }
        public string NgayLayMauDisplay => NgayLayMau != null ? NgayLayMau.Value.ApplyFormatDateTimeSACH() : null;
        public string NguoiLayMau { get; set; }
        public DateTime? NgayGui { get; set; }
        public string NgayGuiDisplay => NgayGui != null ? NgayGui.Value.ApplyFormatDateTimeSACH() : null;
        public string NguoiGui { get; set; }
        public string SoPhieu { get; set; }
        public long? PhieuGuiMauXetNghiemId { get; set; }
    }

    #endregion

    #region barcode

    public class LayMauXetNghiemBarcodeVo
    {
        public LayMauXetNghiemBarcodeVo()
        {
            BarcodeNumbers = new List<int>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public List<int> BarcodeNumbers { get; set; }
    }


    #endregion

    #region Vo gửi mẫu xét nghiệm

    public class GuiMauXetNghiemVo
    {
        public GuiMauXetNghiemVo()
        {
            NhomMauGuis = new List<LayMauXetNghiemVo>();
        }
        public long? NhanVienGuiMauId { get; set; }
        public DateTime? ThoiDiemGuiMau { get; set; }
        public long? PhongNhanMauId { get; set; }
        public string GhiChu { get; set; }
        public List<LayMauXetNghiemVo> NhomMauGuis { get; set; }
    }

    public class LayMauXetNghiemVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public int? BarcodeNumber { get; set; }
        public string BarcodeId { get; set; }
    }

    public class InPhieuDGuimauXetNghiemVo
    {
        public long PhieuGuiMauId { get; set; }
        public string HostingName { get; set; }
        public bool HasHeader { get; set; }
    }

    public class ThongTinInPhieuGuiMauXetNghiemVo
    {
        public string Header { get; set; }
        public string TenNguoiGuiMau { get; set; }
        public string BoPhan { get; set; }
        public string GhiChu { get; set; }
        public string GuiToiBoPhan { get; set; }
        public int TongSoLuongMau { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string DanhSachMau { get; set; }

    }

    public class ThongTinInPhieuGuiMauXetNghiemChiTietVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string TenThuoc { get; set; }
        public string DVT { get; set; }
        public double SLYeuCau { get; set; }
        public double SLThucXuat { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
    }
    #endregion
}
