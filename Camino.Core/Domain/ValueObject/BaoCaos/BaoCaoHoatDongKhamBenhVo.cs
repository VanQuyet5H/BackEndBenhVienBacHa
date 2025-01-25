using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoHoatDongKhamBenhTimKiemVo
    {
        public BaoCaoHoatDongKhamBenhTimKiemVo()
        {
            NhomDichVuKhamBenhIds = new List<long>();
        }
        public bool DangKham { get; set; }
        public bool DaHoanThanh { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }

        public DateTime TuNgayTimKiemData { get; set; }
        public DateTime DenNgayTimKiemData { get; set; }

        public List<long> NhomDichVuKhamBenhIds { get; set; }
    }

    public class ChiTietThucHienDichVuGridVo : GridItem
    {
        public bool LaKhamSucKhoe { get; set; }
        public bool LaKhamBenh { get; set; }
        public bool LaKhamTiemChung { get; set; }
        public bool LaKhamSangLoc { get; set; }

        public string MaYeuCauTiepNhan { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string TenDichVu { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiKhamBenh { get; set; }
        public bool DaHoanThanhKhamTiemVacxin { get; set; }
        public bool DuDieuKienTiemChung { get; set; }
        public Enums.TrangThaiTiemChung TrangThaiTiemChung { get; set; }
        public long NoiThucHienId { get; set; }
        public string TenNoiThucHien { get; set; }
        public bool LaNgoaiVien { get; set; }
        public bool CoBHYT { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }

        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }


        public bool LaKhamCapCuu { get; set; }

        public bool LaTreEm => DateHelper.KiemTraDoTuoiTheoNgaySinh(NgaySinh, ThangSinh, NamSinh, 16, false);
        public bool LaTreEmDuoi6Tuoi => DateHelper.KiemTraDoTuoiTheoNgaySinh(NgaySinh, ThangSinh, NamSinh, 6);
        public bool TuVong { get; set; }
        public bool ChuyenVien { get; set; }
        public bool LaNguoiBenhDiaChiKhacHaNoi { get; set; }

        public int TongNguoiBenh { get; set; }

        //Cập nhật 19/07/2022: sửa logic số người khám bệnh tại mục 1b thành đếm theo mã người bệnh
        // logic cũ: đếm theo mã YCTN với dịch vụ khám đầu tiên
        public string MaBN { get; set; }
    }

    public class BaoCaoHoatDongKhamBenhTheoDichVuGridVo : GridItem
    {
        public Enums.NoiDungBaoCaoHoatDongKhamBenh LoaiNoiDung { get; set; }
        public string[] lstLoaiNoiDung => LoaiNoiDung.GetDescription().Split("|");
        public string STT => lstLoaiNoiDung.Length == 2 ? lstLoaiNoiDung[0] : null;
        public string NoiDung => lstLoaiNoiDung.Length == 2 ? lstLoaiNoiDung[1] : lstLoaiNoiDung[0];

        // dùng để xử lý nội dung cha, con
        public bool InDamNoiDung => !string.IsNullOrEmpty(STT) && STT.All(char.IsDigit); // nếu là nội chung cha, thì xử lý in đậm
        public bool LuiNoiDungHienThiVao => STT == null; // nếu là nội dung con, ko có STT thì lùi vào
        public bool LaItemTieuDe { get; set; }

        public int TongSo => KhamCapCuu + KhamNoi + KhamNhi + KhamTaiMuiHong
                             + KhamRangHamMat + KhamMat + KhamNgoai + KhamDaLieu
                             + KhamPhuSan + KhamThamMy + KhamTiemChung + TongSoTheoKhoaPhong;

        public int KhamCapCuu { get; set; }
        public int KhamNoi { get; set; }
        public int KhamNhi { get; set; }
        public int KhamTaiMuiHong { get; set; }
        public int KhamRangHamMat { get; set; }
        public int KhamMat { get; set; }
        public int KhamNgoai { get; set; }
        public int KhamDaLieu { get; set; }
        public int KhamPhuSan { get; set; }
        public int KhamThamMy { get; set; }
        public int KhamTiemChung { get; set; }

        public int TongSoTheoKhoaPhong { get; set; }

        //Cập nhật 19/07/2022: sửa logic số người khám bệnh tại mục 1b thành đếm theo mã người bệnh
        public bool LaDemTongNguoiKhamBenh1b { get; set; }
        public List<NguoiBenhKhamBenhDaDemSoLuongVo> NguoiBenhDaDemSoLuongs { get; set; } = new List<NguoiBenhKhamBenhDaDemSoLuongVo>();
    }

    public class DichVuKhamTheoNhomId
    {
        public DichVuKhamTheoNhomId()
        {
            lstDichVuKhamCapCuuId = new List<long>();
            lstDichVuKhamNoiId = new List<long>();
            lstDichVuKhamNhiId = new List<long>();
            lstDichVuKhamTMHId = new List<long>();
            lstDichVuKhamRHMId = new List<long>();
            lstDichVuKhamMatId = new List<long>();
            lstDichVuKhamNgoaiId = new List<long>();
            lstDichVuKhamDaLieuId = new List<long>();
            lstDichVuKhamPhuSanId = new List<long>();
            lstDichVuKhamThamMyId = new List<long>();
        }
        public List<long> lstDichVuKhamCapCuuId { get; set; }
        public List<long> lstDichVuKhamNoiId { get; set; }
        public List<long> lstDichVuKhamNhiId { get; set; }
        public List<long> lstDichVuKhamTMHId { get; set; }
        public List<long> lstDichVuKhamRHMId { get; set; }
        public List<long> lstDichVuKhamMatId { get; set; }
        public List<long> lstDichVuKhamNgoaiId { get; set; }
        public List<long> lstDichVuKhamDaLieuId { get; set; }
        public List<long> lstDichVuKhamPhuSanId { get; set; }
        public List<long> lstDichVuKhamThamMyId { get; set; }
    }

    public class BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo : BaoCaoHoatDongKhamBenhTheoDichVuGridVo
    {
        public BaoCaoHoatDongKhamBenhTheoKhoaPhongGridVo()
        {
            ThongTinKhamTheoPhongs = new List<ThongTinKhamBenhTheoPhongVo>();
        }
        public List<ThongTinKhamBenhTheoPhongVo> ThongTinKhamTheoPhongs { get; set; }
    }

    public class ThongTinKhamBenhTheoPhongVo
    {
        public long PhongBenhVienId { get; set; }
        public string TenPhongBenhVien { get; set; }
        public int SoLuong { get; set; }
    }

    public class ColumnPhongKhamExcelInfoVo : ThongTinKhamBenhTheoPhongVo
    {
        public string ColumnName { get; set; }
    }

    //Cập nhật 19/07/2022: sửa logic số người khám bệnh tại mục 1b thành đếm theo mã người bệnh
    public class NguoiBenhKhamBenhDaDemSoLuongVo
    {
        public string MaBN { get; set; }
        public Enums.NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh NhomDichVu { get; set; }
    }
}
