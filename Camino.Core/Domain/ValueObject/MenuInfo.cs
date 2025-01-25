namespace Camino.Core.Domain.ValueObject
{
    public class MenuInfo
    {
        #region Internal

        public bool CanViewUser { get; set; }
        public bool CanViewRole { get; set; }
        public bool CanViewQuanLyNhatKyHeThong { get; set; }
        public bool CanViewQuanLyCacCauHinh { get; set; }
        public bool CanViewQuanLyDanhMucNhanVien { get; set; }
        public bool CanViewQuanLyLichSuSMS { get; set; }
        public bool CanViewQuanLyLichSuThongBao { get; set; }
        public bool CanViewQuanLyLichSuEmail { get; set; }
        public bool CanViewQuanLyCacMessagingTemplate { get; set; }
        public bool CanViewQuanLyNoiDungMauXuatRaPdf { get; set; }

        #region DanhMuc
        public bool CanViewDanhMucGiuongBenh { get; set; }
        public bool CanViewTinhTrangGiuongBenh { get; set; }

        #region NhomHeThong
        public bool CanViewDanhMucNgheNghiep { get; set; }
        public bool CanViewDanhMucChucVu { get; set; }
        public bool CanViewDanhMucChucDanh { get; set; }
        public bool CanViewDanhMucVanBangChuyenMon { get; set; }
        public bool CanViewDanhMucQuocGia { get; set; }
        public bool CanViewDanhMucLoaiBenhVien { get; set; }
        public bool CanViewDanhMucCapQuanLyBenhVien { get; set; }
        public bool CanViewDanhMucBenhVien { get; set; }
        public bool CanViewDanhMucLoaiPhongBenh { get; set; }
        //public bool CanViewDanhMucLyDoKhamBenh { get; set; }
        public bool CanViewDanhMucLyDoTiepNhan { get; set; }
        public bool CanViewDanhMucPhuongPhapVoCam { get; set; }

        public bool CanViewCanLamSang { get; set; }

        //public bool CanViewDanhMucNguoiGioiThieu { get; set; }
        public bool CanViewDanhMucNoiGioiThieu { get; set; }

        public bool CanViewPhauThuatThuThuatTheoNgay { get; set; }

        public bool CanViewLichSuPhauThuatThuThuat { get; set; }


        public bool CanViewDanhMucQuanHeThanNhan { get; set; }
        public bool CanViewDanhMucNhomDichVuBenhVien { get; set; }

        public bool CanViewDanhMucDichVuKhamBenh { get; set; }
        public bool CanViewDanhMucChuyenKhoa { get; set; }
        //public bool CanViewDanhMucDichVuCanLamSang { get; set; }
        public bool CanViewDanhMucVatTuYTe { get; set; }
        public bool CanViewDanhMucDichVuKyThuat { get; set; }

        public bool CanViewDanhMucPhamViHanhNghe { get; set; }

        public bool CanViewDanhMucHocViHocHam { get; set; }

        public bool CanViewDanhMucNhomChucDanh { get; set; }
        public bool CanViewDanhMucMauVaChePham { get; set; }
        public bool CanViewDanhMucDonViTinh { get; set; }
        public bool CanViewDanhMucNhaSanXuat { get; set; }
        public bool CanViewDanhMucDuocPham { get; set; }
        public bool CanViewDanhMucDuongDung { get; set; }
        public bool CanViewDanhMucThuocHoacHoatChat { get; set; }
        public bool CanViewDanhMucNhomThuoc { get; set; }
        public bool CanViewDanhMucAdrPhanUngCoHaiCuaThuoc { get; set; }
        public bool CanViewDanhMucDichVuGiuong { get; set; }
        public bool CanViewDanhMucDichVuGiuongTaiBenhVien { get; set; }
        public bool CanViewDanhMucDichVuKhamBenhTaiBenhVien { get; set; }
        public bool CanViewDanhMucDichVuKyThuatTaiBenhVien { get; set; }
        public bool CanViewDanhMucNhomDichVuKyThuat { get; set; }
        public bool CanViewDanhMucNhaThau { get; set; }
        public bool CanViewDanhMucKhoDuocPham { get; set; }

        public bool CanViewDanhMucKhoDuocPhamViTri { get; set; }

        public bool CanViewDanhMucDinhMucDuocPhamTonKho { get; set; }

        public bool CanViewDanhMucHopDongThauDuocPham { get; set; }
        public bool CanViewDanhMucDuocPhamBenhVien { get; set; }
        public bool CanViewDanhMucNhomVatTuYTe { get; set; }
        public bool CanViewDanhMucVatTuYTeTaiBenhVien { get; set; }
        public bool CanViewDanhMucChiSoXetNghiem { get; set; }
        public bool CanViewDanhMucChuanDoanHinhAnh { get; set; }
        public bool CanViewDanhMucTrieuChung { get; set; }
        public bool CanViewDanhMucChuanDoan { get; set; }
        public bool CanViewDanhMucNhomChanDoan { get; set; }
        public bool CanViewToaThuocMau { get; set; }

        public bool CanViewBaoCaoChiTietDoanhThuTheoBacSi { get; set; }
        public bool CanViewBaoCaoChiTietDoanhThuTheoKhoaPhong { get; set; }
        public bool CanViewBaoCaoThuVienPhiBenhNhan { get; set; }
        //public bool CanViewBaoCaoNoiTruNgoaiTru { get; set; }
        public bool CanViewBaoCaoTongHopDoanhThuTheoBacSi { get; set; }
        public bool CanViewBaoCaoTongHopDoanhThuTheoKhoaPhong { get; set; }
        //public bool CanViewTheoDoiTinhHinhThanhToanVienPhi { get; set; }
        public bool CanViewBaoCaoDanhSachThuTienVienPhi { get; set; }
        public bool CanViewGuiBaoHiemYTe { get; set; }
        public bool CanViewDanToc { get; set; }

        public bool CanViewLichSuCanLamSang { get; set; }

        public bool CanViewLichSuGuiBHYT { get; set; }
        public bool CanViewQuanLyICD { get; set; }

        public bool CanViewBaoCaoBangKeChiTietTTCN { get; set; }
        public bool CanViewBaoCaoTheKho { get; set; }
        public bool CanViewBaoCaoTonKho { get; set; }
        public bool CanViewBaoCaoDoanhThuTheoNhomDichVu { get; set; }
        public bool CanViewBaoCaoBienBanKiemKeKT { get; set; }
        public bool CanViewBaoCaoBangKePhieuXuatKho { get; set; }
        public bool CanViewBaoCaoTinhHinhNhapTuNhaCungCap { get; set; }
        public bool CanViewBaoCaoSoChiTietVatTuHangHoa { get; set; }
        public bool CanViewBaoCaoDoanhThuKhamDoanTheoNhomDV { get; set; }
        public bool CanViewBaoCaoSoLieuThoiGianSuDungDV { get; set; }
        public bool CanViewBaoCaoDoanhThuKhamDoanTheoKP { get; set; }
        public bool CanViewBaoCaoChiTietHoaHongCuaNguoiGT { get; set; }
        public bool CanViewBaoCaoCamKetSuDungThuocNgoaiBHYT { get; set; }
        public bool CanViewBaoCaoBangKeGiaoHoaDonSangPKT { get; set; }
        public bool CanViewBaoCaoHoatDongNoiTru { get; set; }
        public bool CanViewBaoCaoBienBanKiemKeDuocVT { get; set; }
        public bool CanViewBaoCaoThongKeKSK { get; set; }
        public bool CanViewBaoCaoNguoiBenhDenKham { get; set; }
        public bool CanViewBaoCaoNguoiBenhDenLamDVKT { get; set; }
        public bool CanViewBaoCaoTraCuuDuLieu { get; set; }
        public bool CanViewBaoCaoTongHopCongNoChuaThanhToan { get; set; }
        public bool CanViewBaoCaoBangKeChiTietTheoNguoiBenh { get; set; }
        public bool CanViewBaoCaoDichVuPhatSinhNgoaiGoiCuaKeToan { get; set; }
        public bool CanViewBangThongKeTiepNhanNoiTruVaNgoaiTru { get; set; }
        public bool CanViewBaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong { get; set; }
        public bool CanViewBaoCaoDoanhThuKhamDoanTheoNhomDichVu { get; set; }
        public bool CanViewBaoCaoBenhNhanRaVienNoiTru { get; set; }

        public bool CanViewThongKeThuocTheoBacSi { get; set; }
        public bool CanViewThongKeBSKeDonTheoThuoc { get; set; }
        public bool CanViewThongKeCacDichVuChuaLayLenBienLaiThuTien { get; set; }
        public bool CanViewDanhSachBARaVienChuaXacNhanHoanTatChiPhi { get; set; }
        public bool CanViewDanhSachThuVienPhiNoiTruVaNgoaiTruChuaHoan { get; set; }
        public bool CanViewDanhSachXuatChungTuExcel { get; set; }
        public bool CanViewDanhSachLichSuHuyBanThuoc { get; set; }
        public bool CanViewBaoCaoKetQuaKhamChuaBenhKT { get; set; }
        public bool CanViewBaoCaoHoatDongNoiTruChiTiet { get; set; }
        public bool CanViewBaoCaoTinhHinhBenhTatTuVong { get; set; }
        public bool CanViewDanhMucBenhVaNhomBenh { get; set; }

        #endregion NhomHeThong

        #region NhomKhoaPhong
        public bool CanViewDanhMucKhoaPhong { get; set; }

        public bool CanViewDanhMucKhoaPhongNhanVien { get; set; }

        public bool CanViewDanhMucKhoaPhongPhongKham { get; set; }

        public bool CanViewXacNhanBhytDaHoanThanh { get; set; }
        #endregion

        #region LichPhanCong
        public bool CanViewDanhMucLichPhanCongNgoaiTru { get; set; }
        #endregion

        #region NhomNhanVien
        public bool CanViewDanhMucNhanVien { get; set; }
        #endregion

        #region NhomPhongBan
        #endregion NhomPhongBan
        #endregion DanhMuc
        //public bool CanViewYeuCauKhamBenh { get; set; }
        public bool CanViewDanhSachChoKham { get; set; }
        public bool CanViewNhapKhoDuocPham { get; set; }
        public bool CanViewXuatKhoDuocPham { get; set; }
        public bool CanViewGoiDichVuMarketing { get; set; }
        public bool CanViewGoiDvChuongTrinhMarketing { get; set; }
        public bool CanViewDuocPhamTonKho { get; set; }
        public bool CanViewDuocPhamSapHetHan { get; set; }
        public bool CanViewDuocPhamDaHetHan { get; set; }
        public bool CanViewKhamBenh { get; set; }
        public bool CanViewDoiTuongUuDaiDichVuKyThuat { get; set; }
        public bool CanViewDoiTuongUuDaiDichVuKhamBenh { get; set; }
        public bool CanViewXacNhanBHYT { get; set; }
        public bool CanViewThuNgan { get; set; }
        public bool CanViewQuayThuoc { get; set; }
        public bool CanViewLoiDan { get; set; }
        public bool CanViewBenhNhan { get; set; }
        public bool CanViewBaoCaoThuChi { get; set; }
        public bool CanViewLichSuTiepNhan { get; set; }
        public bool CanViewLichSuKhamBenh { get; set; }
        public bool CanViewLichSuXacNhanBHYT { get; set; }
        public bool CanViewLichSuThuNgan { get; set; }
        public bool CanViewLichSuQuayThuoc { get; set; }
        public bool CanViewYeuCauTiepNhan { get; set; }
        //public bool CanViewXacNhanThuNganDaHoanThanh  { get; set; }

        public bool CanViewDuyetNhapKho { get; set; }
        public bool CanViewDuyetNhapKhoVatTu { get; set; }
        public bool CanViewNhapKhoVatTu { get; set; }
        public bool CanViewLinhTrucTiepDuocPham { get; set; }
        public bool CanViewDanhSachLichSuBanThuoc { get; set; }

        public bool CanViewDanhMucHopDongThauVatTu { get; set; }

        public bool CanViewDanhMucDinhMucVatTuTonKho { get; set; }

        public bool CanViewDanhMucDuocPhamBenhVienPhanNhom { get; set; }

        public bool CanViewDanhMucCongTyBhtn { get; set; }
        public bool CanViewDanhMucCongTyUuDai { get; set; }

        //        public bool CanViewDanhSachCanLinhTrucTiepDuocPham { get; set; }
        //        public bool CanViewDanhSachCanLinhTrucTiepVatTu { get; set; }
        //        public bool CanViewDanhSachCanLinhBuDuocPham { get; set; }
        //        public bool CanViewDanhSachCanLinhBuVatTu { get; set; }

        public bool CanViewDuyetNhapKhoDuocPham { get; set; }
        public bool CanViewDanhSachYeuCauLinhDuocPham { get; set; }
        public bool CanViewTaoYeuCauLinhThuongDuocPham { get; set; }
        public bool CanViewTaoYeuCauLinhBuDuocPham { get; set; }
        public bool CanViewTaoYeuCauLinhTrucTiepDuocPham { get; set; }
        public bool CanViewDuyetYeuCauLinhDuocPham { get; set; }
        public bool CanViewDanhSachYeuCauLinhVatTu { get; set; }
        public bool CanViewTaoYeuCauLinhThuongVatTu { get; set; }
        public bool CanViewTaoYeuCauLinhBuVatTu { get; set; }
        public bool CanViewTaoYeuCauLinhTrucTiepVatTu { get; set; }
        public bool CanViewDuyetYeuCauLinhVatTu { get; set; }
        public bool CanViewXuatKhoVatTu { get; set; }
        public bool CanViewYeuCauHoanTraDuocPham { get; set; }
        public bool CanViewYeuCauHoanTraVatTu { get; set; }
        //public bool CanViewDanhSachDuyetYeuCauHoanTraDuocPham { get; set; }
        //public bool CanViewDanhSachDuyetYeuCauHoanTraVatTu { get; set; }
        public bool CanViewDuyetYeuCauHoanTraDuocPham { get; set; }
        public bool CanViewDuyetYeuCauHoanTraVatTu { get; set; }
        public bool CanViewVatTuTonKho { get; set; }
        public bool CanViewVatTuSapHetHan { get; set; }
        public bool CanViewVatTuDaHetHan { get; set; }
        public bool CanViewQuaTangMarketing { get; set; }
        public bool CanViewGoiDichVuNhomThuongDung { get; set; }


        public bool CanViewXuatKhoDuocPhamKhac { get; set; }

        public bool CanViewXuatKhoVatTuKhac { get; set; }

        public bool CanViewKhamBenhDangKham { get; set; }
        public bool CanViewNhapKhoMarketing { get; set; }
        public bool CanViewXuatKhoMarketing { get; set; }
        public bool CanViewVoucherMarketing { get; set; }


        public bool CanViewDanhSachMarketing { get; set; }
        public bool CanViewCongNoBhtn { get; set; }
        public bool CanViewDanhSachYeuCauDuTruMuaDuocPham { get; set; }
        public bool CanViewDanhSachTongHopDuTruMuaDuocPhamTaiKhoa { get; set; }
        public bool CanViewDanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc { get; set; }
        public bool CanViewDanhSachTongHopDuTruMuaDuocPhamTaiGiamDoc { get; set; }
        public bool CanViewBaoCaoCongNoCongTyBhtn { get; set; }
        public bool CanViewKyDuTru { get; set; }
        public bool CanViewDanhSachYeuCauDuTruMuaVatTu { get; set; }
        public bool CanViewDanhSachTongHopDuTruMuaVatTuTaiKhoa { get; set; }
        public bool CanViewDanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc { get; set; }
        public bool CanViewDanhSachTongHopDuTruMuaVatTuTaiGiamDoc { get; set; }
        public bool CanViewDuyetKetQuaXetNghiem { get; set; }
        public bool CanViewDuyetYeuCauChayLaiXetNghiem { get; set; }
        public bool CanViewKetQuaXetNghiem { get; set; }
        public bool CanViewChiSoXetNghiem { get; set; }
        public bool CanViewThietBiXetNghiem { get; set; }
        public bool CanViewGoiMauXetNghiem { get; set; }
        public bool CanViewNhanMauXetNghiem { get; set; }
        public bool CanViewLayMauXetNghiem { get; set; }
        public bool CanViewDanhSachDieuTriNoiTru { get; set; }
        public bool CanViewTiepNhanNoiTru { get; set; }
        public bool CanViewTongHopYLenh { get; set; }
//        public bool CanViewKetQuaSieuAm { get; set; }
//        public bool CanViewKetQuaXQuang { get; set; }
//        public bool CanViewKetQuaNoiSoi { get; set; }
//        public bool CanViewKetQuaDienTim { get; set; }
        public bool CanViewNhapKhoMau { get; set; }
        public bool CanViewDuyetNhapKhoMau { get; set; }
        public bool CanViewLuuTruHoSo { get; set; }
        public bool CanViewTraThuocTuBenhNhan { get; set; }
        public bool CanViewTraVatTuTuBenhNhan { get; set; }

        public bool CanViewXacNhanBhytNoiTru { get; set; }

        public bool CanViewDuyetTraThuocTuBenhNhan { get; set; }

        public bool CanViewDuyetTraVatTuTuBenhNhan { get; set; }
        public bool CanViewCongNoBenhNhan { get; set; }

        public bool CanViewBaoCaoLuuTruHoSoBenhAn { get; set; }
        public bool CanViewBaoCaoBSDanhSachKhamNgoaiTru { get; set; }
        public bool CanViewBaoCaoXuatNhapTon { get; set; }
        public bool CanViewBaoCaoTiepNhanBenhNhanKham { get; set; }
        public bool CanViewBaoCaoDoanhThuNhaThuoc { get; set; }

        public bool CanViewBaoCaoHoatDongKhoaKhamBenh { get; set; }

        public bool CanViewBaoCaoThucHienCls { get; set; }

        public bool CanViewDanhSachBenhNhanPhauThuat { get; set; }

        public bool CanViewBaoCaoKetQuaKhamChuaBenh { get; set; }
        public bool CanViewBaoCaoVienPhiThuTien { get; set; }
        public bool CanViewBaoCaoThongKeDonThuoc { get; set; }
        public bool CanViewKhamDoanLichSuTiepNhanKhamSucKhoe { get; set; }
       
        public bool CanViewKhamDoanCongTy { get; set; }
        public bool CanViewKhamDoanChiSoSinhTon { get; set; }
        public bool CanViewKhamDoanHopDongKham { get; set; }
        public bool CanViewKhamDoanKetLuanCanLamSangKhamSucKhoeDoan { get; set; }
        public bool CanViewKhamDoanTiepNhan { get; set; }
        public bool CanViewKhamDoanGoiKhamSucKhoe { get; set; }

        public bool CanViewKhamDoanYeuCauNhanSuKhamSucKhoe { get; set; }
        public bool CanViewKhamDoanKetLuanKhamSucKhoeDoan { get; set; }
        public bool CanViewKhamDoanDuyetYeuCauNhanSuKhamSucKhoeKhth { get; set; }
        public bool CanViewKhamDoanDuyetYeuCauNhanSuKhamSucKhoePhongNhanSu { get; set; }
        public bool CanViewKhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc { get; set; }
        public bool CanViewKhamDoanKhamBenh { get; set; }
        public bool CanViewKhamDoanKhamBenhTatCaPhong { get; set; }

        public bool CanViewDanhMucCheDoAn { get; set; }
        public bool CanViewCapNhatDuocPhamTonKho { get; set; }
        public bool CanViewCapNhatVatTuTonKho { get; set; }
        public bool CanViewBaoCaoLuuKetQuaXetNghiemHangNgay { get; set; }
        public bool CanViewBaoCaoXNTVatTu { get; set; }
        public bool CanViewBaoCaoSoXetNghiemSangLocHiv { get; set; }
        public bool CanViewBaoCaoTongHopSoLuongXetNghiem { get; set; }
        public bool CanViewBaoCaoBenhNhanLamXetNghiem { get; set; }
        public bool CanViewDanhSachDieuChuyenNoiBoDuocPham { get; set; }
        public bool CanViewDanhSachDuyetDieuChuyenNoiBoDuocPham { get; set; }
        public bool CanViewBaoCaoBenhNhanKhamNgoaiTru { get; set; }
        public bool CanViewBaoCaoSoLuongThuThuat { get; set; }
        public bool CanViewBaoCaoSoPhucTrinhPhauThuatThuThuat { get; set; }
        public bool CanViewBaoCaoHoatDongClsTheoKhoa { get; set; }
        public bool CanViewBaoCaoSoThongKeCls { get; set; }
        public bool CanViewTaoBenhAnSoSinh { get; set; }
        public bool CanViewTuDienDichVuKyThuat { get; set; }
        public bool CanViewTiemChungKhamSangLoc { get; set; }
        public bool CanViewTiemChungThucHienTiem { get; set; }
        public bool CanViewTiemChungLichSuTiem { get; set; }
        public bool CanViewBaoCaoDichVuTrongGoiKhamDoan { get; set; }
        public bool CanViewBaoCaoDichVuNgoaiGoiKhamDoan { get; set; }

        public bool CanViewBaoCaoTongHopKetQuaKSK { get; set; }
        public bool CanViewBaoCaoHieuQuaCongViec { get; set; }
        public bool CanViewBaoCaoTiepNhanBenhPham { get; set; }
        public bool CanViewBaoCaoTonKhoXN { get; set; }
        public bool CanViewBaoCaoTonKhoKT { get; set; }
        public bool CanViewBaoCaoTinhHinhTraNCC { get; set; }
        public bool CanViewBaoCaoTinhHinhNhapNCCChiTiet { get; set; }
        public bool CanViewBaoCaoDuocChiTietXuatNoiBo { get; set; }
        public bool CanViewBaoCaoChiTietMienPhiTronVien{ get; set; }
        public bool CanViewBaoCaoTongHopDoanhThuThaiSanDaSinh { get; set; }
        public bool CanViewBaoCaoTongHopDangKyGoiDichVu { get; set; }
        public bool CanViewBaoCaoBangKeXuatThuocTheoBenhNhan { get; set; }
        public bool CanViewBaoCaoHoatDongCls { get; set; }
        public bool CanViewBaoCaoThuocSapHetHanDung { get; set; }
        public bool CanViewBaoCaoKTNhapXuatTonChiTiet { get; set; }
        public bool CanViewBaoCaoDuocTinhHinhXuatNoiBo { get; set; }
        public bool CanViewBaoCaoHoatDongKhamDoan { get; set; }

        #region lịch sử khám chữa bệnh
        public bool CanViewLichSuKhamChuaBenh { get; set; }
        #endregion
        public bool CanViewBaoCaoKSKChuyenKhoa { get; set; }
        public bool CanViewBaoCaoHoatDongKhamBenhTheoDichVu { get; set; }
        public bool CanViewBaoCaoHoatDongKhamBenhTheoKhoaPhong { get; set; }
        public bool CanViewBaoCaoThuVienPhiChuaHoan { get; set; }
        public bool CanViewBangKeThuocVatTuPhauThuat { get; set; }
        public bool CanViewBaoCaoTonKhoVatTuYTe { get; set; }
        public bool CanViewBaoCaoTheKhoVatTuYTe { get; set; }
        public bool CanViewKhamDoanThongKeSoNguoiKhamSucKhoeLSCLS { get; set; }
        public bool CanViewDanhMucDichVuKyThuatBenhVien { get; set; }
        public bool CanViewDanhMucDichVuKhamBenhBenhVien { get; set; }

        //public bool CanViewDanhSachGayBenhAn { get; set; }
        #region Bệnh án điện tử
        public bool CanViewDanhMucGayBenhAn { get; set; }
        public bool CanViewBenhAnDienTu { get; set; }
        #endregion
        //public bool CanViewDanhSachGayBenhAn { get; set; }

        public bool CanViewDSXNNgoaiTruVaNoiTruBHYT { get; set; }
        public bool CanViewBaoCaoNhapXuatTon { get; set; }

        public bool CanViewDanhSachDonThuocChoCapThuocBHYT { get; set; }
        public bool CanViewLichSuXuatThuocCapThuocBHYT { get; set; }
        public bool CanViewMoLaiBenhAn { get; set; }

        public bool CanViewNhapVatTuThuocNhomKSNK { get; set; }
        public bool CanViewXuatKhoVatTuThuocNhomKSNK { get; set; }
        public bool CanViewXuatKhoKhacVatTuThuocNhomKSNK { get; set; }

        public bool CanViewYeuCauDuTruMuaNhomKSNK { get; set; }
        public bool CanViewTHDTMuaTaiKSNK { get; set; }
        public bool CanViewTHDTMuaTaiHanhChinh { get; set; }
        public bool CanViewTHDTMuaTaiGiamDoc { get; set; }

        public bool CanViewYeuCauHoanTraKSNK { get; set; }
        public bool CanViewDuyetYeuCauHoanTraKSNK { get; set; }

        public bool CanViewDanhSachYeuCauLinhKSNK { get; set; }
        public bool CanViewTaoYeuCauLinhThuongKSNK { get; set; }
        public bool CanViewTaoYeuCauLinhBuKSNK { get; set; }
        public bool CanViewDuyetYeuCauLinhKSNK { get; set; }

        public bool CanViewBaoCaoXNXuatNhapTonKhoXetNghiem { get; set; }
        public bool CanViewBaoCaoXNPhieuNhapHoaChat { get; set; }
        public bool CanViewBaoCaoXNPhieuXuatHoaChat { get; set; }

        public bool CanViewDSThanhToanChiPhiKCBNgoaiTru { get; set; }
        public bool CanViewDSThanhToanChiPhiKCBNoiTru { get; set; }

        public bool CanViewQuanLyNgayLe { get; set; }
        public bool CanViewQuanLyLichLamViec { get; set; }

        public bool CanViewThongKeDichVuKhamSucKhoe { get; set; }
        public bool CanViewDanhMucQuanLyHDPP { get; set; }

        public bool CanViewDanhMucCauHinhThuePhong { get; set; }
        public bool CanViewDanhMucLichSuThuePhong { get; set; }
        
        public bool CanViewDayLenCongGiamDinh7980a { get; set; }
        public bool CanViewGiamDinhBHYT7980aXuatChoKeToan { get; set; }
        public bool CanViewCauHinhNguoiDuyetTheoNhomDichVu { get; set; }

        public bool CanViewBaoCaoTinhHinhNhapNhaCungCapChiTiet { get; set; }
        public bool CanViewBaoCaoTinhHinhTraNhaCungCapChiTiet { get; set; }

        public bool CanViewVTYTTinhHinhTraNCC { get; set; }
        public bool CanViewDanhMucLoaiGiaDichVu { get; set; }

        public bool CanViewBaoCaoKeToanBangKeChiTietNguoiBenh { get; set; }
        public bool CanViewVTYTBaoCaoChiTietXuatNoiBo { get; set; }
        public bool CanViewKHTHBaoCaoThongKeSLThuThuat { get; set; }
        public bool CanViewVTYTBaoCaoChiTietHoanTraNoiBo { get; set; }

        public bool CanViewBCDTKhamDoanTheoNhomDVDGThucTe { get; set; }
        public bool CanViewBCDTKhamDoanTheoKhoaPhongDGThucTe { get; set; }
        public bool CanViewBCDTChiaTheoPhong { get; set; }
        public bool CanViewBCDTTongHopDoanhThuTheoNguonBenhNhan { get; set; }
        public bool CanViewDanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong { get; set; }

        public bool CanViewDanhMucDonViHanhChinh { get; set; }

        #endregion Internal
    }
    public class PortalMenuInfo
    {

        #region External
        #endregion External
    }
}
