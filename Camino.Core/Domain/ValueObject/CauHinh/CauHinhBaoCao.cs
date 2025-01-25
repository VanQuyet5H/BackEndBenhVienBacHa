using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhBaoCao : ISettings
    {
        public CauHinhBaoCao()
        {
            SoGiuongKeHoach = 35;
            NhomXNHuyetHoc = 205;
            NhomXNHoaSinh = 204;
            NhomXNViSinh = 208;
            NhomXNNuocTieu = 252;
            NhomXNSinhHocPhanTu = 246;
            NhomXNTeBaoGiaiPhauBenh = 113;

            NhomChanDoanHinhAnh = 3;
            NhomSieuAm = 202;
            NhomXQuangThuong = 226;
            NhomXQuangSoHoa = 227;
            NhomCTScanner = 230;
            NhomMRI = 231;
            NhomDoLoangXuong = 250;

            NhomThamDoChucNang = 4;
            NhomDienTim = 248;
            NhomDienNao = 249;
            NhomNoiSoi = 247;
            NhomNoiSoiTMH = 241;
            NhomDoHoHap = 251;
            TenDichVuXQuangPanorama = "Panorama";
            TenDichVuCTConebeam = "CT Conebeam";

            DuocPhamBenhVienNhomHoaChat = 43;
            HinhThucDenGioiThieuId = 2;
            HinhThucDenCBNVId = 20;
            NoiGioiThieuThamMy = "thẩm mỹ";
            NoiGioiThieuThamMyBsTu = "thẩm mỹ bs tú";
            NoiGioiThieuVietDuc = "việt đức";
            NhomDichVuBenhVienSuatAnId = 200;
            NhomDichVuBenhVienRangHamMatIds = "223;242";
            DichVuKhamRangHamMatIds = "6";
            KhungGioBaoCaoKetQuaKhamChuaBenh = (16 - 24) * 60 * 60 + 30 * 60;//16h30' ngày hôm trước
            HoatDongNoiTruChiTietKhoaPhongIds = "5;7;6;8;9";
            HinhThucDenTuDenId = 1;
            NoiGioiThieuTachDoanhThuDPVTIds = "468";
        }
        public int DuocPhamSapHetHanNgayHetHan { get; set; }
        public int VatTuSapHetHanNgayHetHan { get; set; }
        public int SoGiuongKeHoach { get; set; }
        public long NhomXNHuyetHoc { get; set; }
        public long NhomXNHoaSinh { get; set; }
        public long NhomXNViSinh { get; set; }
        public long NhomXNNuocTieu { get; set; }
        public long NhomXNSinhHocPhanTu { get; set; }
        public long NhomXNTeBaoGiaiPhauBenh { get; set; }

        public long NhomChanDoanHinhAnh { get; set; }
        public long NhomSieuAm { get; set; }
        public long NhomXQuangThuong { get; set; }
        public long NhomXQuangSoHoa { get; set; }
        public long NhomCTScanner { get; set; }
        public long NhomMRI { get; set; }
        public long NhomDoLoangXuong { get; set; }

        public long NhomThamDoChucNang { get; set; }
        public long NhomDienTim { get; set; }
        public long NhomDienNao { get; set; }
        public long NhomNoiSoi { get; set; }
        public long NhomNoiSoiTMH { get; set; }
        public long NhomDoHoHap { get; set; }
        public string TenDichVuXQuangPanorama { get; set; }
        public string TenDichVuCTConebeam { get; set; }
        public long DuocPhamBenhVienNhomHoaChat { get; set; }

        public long HinhThucDenTuDenId { get; set; }
        public long HinhThucDenGioiThieuId { get; set; }
        public long HinhThucDenCBNVId { get; set; }
        public string NoiGioiThieuThamMyBsTu { get; set; }
        public string NoiGioiThieuThamMy { get; set; }
        public string NoiGioiThieuVietDuc { get; set; }
        public long NhomDichVuBenhVienSuatAnId { get; set; }
        public string NhomDichVuBenhVienRangHamMatIds { get; set; }
        public string DichVuKhamRangHamMatIds { get; set; }
        public int KhungGioBaoCaoKetQuaKhamChuaBenh { get; set; }
        public string HoatDongNoiTruChiTietKhoaPhongIds { get; set; }
        public string NoiGioiThieuTachDoanhThuDPVTIds { get; set; }
        public long TinhHaNoi { get; set; }
    }
}
