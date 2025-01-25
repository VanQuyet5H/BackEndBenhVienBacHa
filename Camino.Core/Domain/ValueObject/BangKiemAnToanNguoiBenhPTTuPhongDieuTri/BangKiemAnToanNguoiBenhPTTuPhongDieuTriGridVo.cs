using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BangKiemAnToanNguoiBenhPTTuPhongDieuTri
{
    public class BangKiemAnToanNguoiBenhPTTuPhongDieuTriGridVo : GridItem
    {
        public string ThongTinHoSo { get; set; }
        public DateTime Ngay { get; set; }
        public string NgayString => Ngay.ApplyFormatDate();
        public string HoTenPTV { get; set; }
    }
    public class PartObject 
    {
        public string HoTenPTV { get; set; }
        public DateTime NgayNhan { get; set; }
        public string NgayNhanUTC { get; set; }
    }
    public class BangKiemAnToanNguoiBenhPTTuPhongDieuTriGrid :GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyGridVo> ListFile { get; set; }
    }
    public class FileChuKyGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class InBangKiemBenhNhanPhauThuatVeKhoaDieuTri
    {
        public bool? TinhTao { get; set; }
        public bool? KichThichVatVa { get; set; }
        public bool? DHSinhTonOnDinh { get; set; }
        public bool? NghiNgoChayMau { get; set; }
        public bool? SuyHoHap { get; set; }
        public bool? NonNac { get; set; }
        public bool? CauBangQuang { get; set; }
        public bool? VanTimTrenDa { get; set; }
        public bool? DauNhieu { get; set; }
        public bool? TruyenDich { get; set; }
        public string ViTri { get; set; }
        public bool? LuuThong { get; set; }
        public bool? Sach { get; set; }
        public bool? OngThongDaDay { get; set; }
        public bool? OngThongTieu { get; set; }
        public bool? DanLuu { get; set; }
        public bool? ApLuc { get; set; }
        public string BangKhoValue { get; set; }
        public string ThamDichValue { get; set; }
        public bool? TuTrangBenhNhan { get; set; }
        public bool? BAKhoaHoanThien { get; set; }

        public string HoTenPTV { get; set; }
        public string NoiCongTac { get; set; }
        public string CheDoAnUong { get; set; }
        public bool? GiaiPhauBenh { get; set; }
        public long? SoMauBenhPham { get; set; }
        public string KetQuaFilm { get; set; }
        public string XetNghiemCanLam { get; set; }
        public bool? DaKyChonPhong { get; set; }
        public string CheDoChamSoc { get; set; }
        public string ChiDinhTheoDoi { get; set; }
      
        public DateTime NgayNhan {get;set;}
        public  string NguoiGiao { get;set;}
        public string NguoiNhan { get;set;}
        public int? SoLuongMauSacOngThongTieu { get; set; }
        public int? SoLuongMauSacDaDay { get; set; }
        public string MauSacOngThongTieu { get; set; }
        public string MauSacDaDay { get; set; }
        public string ViTriDanLuu { get; set; }

        public int? SoLuongMauSacDich { get; set; }
        public string MauSacDich { get; set; }
        public string NhapTuTrang { get; set; }


        public bool? KiemTraTruocKhiLenPM { get; set; }
        public bool KiemTraTruocKhiRachDa { get; set; }
        public bool? BV { get; set; }
        public bool? HT { get; set; }
        public bool? ThuocDangDungValue{ get; set; }
        public bool? ThuocBanGiaoValue { get; set; }
        public bool? Cap1{ get; set; }
        public bool? Cap2{ get; set; }
        public bool? Cap3{ get; set; }
        public string CheDoChamSocString { get; set; }


        public bool? BangTheoDoiGMHS { get; set; }
        public bool? BangTheoDoiHoiTinh { get; set; }
        public bool? PhieuDemGac { get; set; }
        public bool? PhieuTheoDoiTeNMC { get; set; }
        public bool? PhieuVTNgoai { get; set; }
        public bool? PhieuVTTHGMPT { get; set; }
        public bool? SoLuongFilm { get; set; }
        public string QuanAoVay { get; set; }
    }
}
