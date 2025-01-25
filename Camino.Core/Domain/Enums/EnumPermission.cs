using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum DocumentType
        {
            None = 0,
            #region Internal
            [Description("Quản Lý Người Dùng")]
            User = 1,
            [Description("Phân Quyền Người Dùng")]
            Role = 2,
            [Description("Quản Lý Nhật Ký Hệ Thống")]
            QuanLyNhatKyHeThong = 3,
            [Description("Thông Số Mặc Định")]
            QuanLyCacCauHinh = 4,
            //[Description("Quản Lý Nhân Viên")]
            //QuanLyNhanVien = 5,
            [Description("Lịch Sử Sms")]
            QuanLyLichSuSMS = 6,
            [Description("Quản Lý Lịch Sử Email")]
            QuanLyLichSuEmail = 7,
            [Description("Quản Lý Lịch Sử Thông Báo")]
            QuanLyLichSuThongBao = 8,
            [Description("Quản Lý Nội Dung Mẫu Gửi Email, Sms, Notification")]
            QuanLyCacMessagingTemplate = 9,
            [Description("Quản Lý Nội Dung Mẫu Xuất Ra Pdf")]
            QuanLyNoiDungMauXuatRaPdf = 10,


            #region DanhMuc
            #region NhomHeThong
            [Description("Danh Mục Nghề Nghiệp")] DanhMucNgheNghiep = 11,
            [Description("Danh Mục Chức Vụ")] DanhMucChucVu = 12,
            [Description("Danh Mục Chức Danh")] DanhMucChucDanh = 13,
            [Description("Danh Mục Văn Bằng Chuyên Môn")] DanhMucVanBangChuyenMon = 14,
            [Description("Danh Mục Loại Bệnh Viện")] DanhMucLoaiBenhVien = 15,
            [Description("Danh Mục Cấp Quản Lý Bệnh Viện")] DanhMucCapQuanLyBenhVien = 16,
            [Description("Danh Mục Tên Bệnh Viện")] DanhMucBenhVien = 17,
            [Description("Danh Mục Loại Phòng Bệnh")] DanhMucLoaiPhongBenh = 18,
            //[Description("Danh Mục Loại Lý Do Khám Bệnh")] DanhMucLyDoKhamBenh = 19,
            [Description("Danh Mục Quan Hệ Thân Nhân")] DanhMucQuanHeThanNhan = 20,
            [Description("Danh Mục Dịch Vụ Khám Bệnh")] DanhMucDichVuKhamBenh = 21,
            [Description("Danh Mục Chuyên Khoa")] DanhMucChuyenKhoa = 22,
            //[Description("Danh Mục Dịch Vụ Cận Lâm Sàng")] DanhMucDichVuCanLamSang = 23,
            [Description("Danh Mục Vật Tư Y tế")] DanhMucVatTuYTe = 24,
            [Description("Danh Mục Dịch Vụ Kỹ Thuật")] DanhMucDichVuKyThuat = 25,
            [Description("Danh Mục Phạm Vi Hành Nghề")] DanhMucPhamViHanhNghe = 28,
            [Description("Danh Mục Học Vị Học Hàm")] DanhMucHocViHocHam = 29,
            [Description("Danh Mục Nhóm Chức Danh")] DanhMucNhomChucDanh = 30,
            [Description("Danh Mục Máu Và Chế Phẩm")] DanhMucMauVaChePham = 36,
            [Description("Danh Mục Đơn Vị Tính")] DanhMucDonViTinh = 37,
            [Description("Danh Mục Nhà Sản Xuất")] DanhMucNhaSanXuat = 38,
            [Description("Danh Mục Dược Phẩm")] DanhMucDuocPham = 39,
            [Description("Danh Mục Đường Dùng")] DanhMucDuongDung = 40,
            [Description("Danh Mục Thuốc Hoặc Hoạt Chất")] DanhMucThuocHoacHoatChat = 41,
            [Description("Danh Mục Nhóm Thuốc")] DanhMucNhomThuoc = 42,
            [Description("Danh Mục ADR - Phản Ứng Có Hại Của Thuốc")] DanhMucAdrPhanUngCoHaiCuaThuoc = 43,
            [Description("Danh Mục Dịch Vụ Giường")] DanhMucDichVuGiuong = 44,
            [Description("Danh Mục Dịch Vụ Giường Tại Bệnh Viện")] DanhMucDichVuGiuongTaiBenhVien = 45,
            [Description("Danh Mục Dịch Vụ Khám Bệnh Tại Bệnh Viện")] DanhMucDichVuKhamBenhTaiBenhVien = 46,
            [Description("Danh Mục Dịch Vụ Kỹ Thuật Tại Bệnh Viện")] DanhMucDichVuKyThuatTaiBenhVien = 47,
            [Description("Danh Mục Nhóm Dịch Vụ Kỹ Thuật")] DanhMucNhomDichVuKyThuat = 48,
            [Description("Danh Mục Nhà Thầu")] DanhMucNhaThau = 49,
            [Description("Danh Mục Kho Dược Phẩm")] DanhMucKhoDuocPham = 50,
            [Description("Danh Mục Hợp Đồng Thầu Dược Phẩm")] DanhMucHopDongThauDuocPham = 51,
            [Description("Danh Mục Kho Dược Phẩm Vị Trí")] DanhMucKhoDuocPhamViTri = 52,
            [Description("Danh Mục Định Mức Dược Phẩm Tồn Kho")] DanhMucDinhMucDuocPhamTonKho = 53,
            [Description("Danh Mục Dược Phẩm Tại Bệnh Viện")] DanhMucDuocPhamBenhVien = 54,
            [Description("Danh Mục Nhóm Vật Tư Y Tế")] DanhMucNhomVatTuYTe = 57,
            [Description("Danh Mục Vật Tư Y Tế Tại Bệnh Viện")] DanhMucVatTuYTeTaiBenhVien = 58,
            [Description("Danh Mục Chỉ Số Xét Nghiệm")] DanhMucChiSoXetNghiem = 59,
            [Description("Danh Mục Chuẩn Đoán Hình Ảnh")] DanhMucChuanDoanHinhAnh = 60,
            [Description("Danh Mục Triệu Chứng")] DanhMucTrieuChung = 61,
            [Description("Danh Mục Chuẩn Đoán")] DanhMucChuanDoan = 62,
            [Description("Danh Mục Nhóm Chẩn Đoán")] DanhMucNhomChanDoan = 63,
            [Description("Người Bệnh")] BenhNhan = 83,

            [Description("Lịch Sử Tiếp Nhận")] LichSuTiepNhan = 84,
            [Description("Lịch Sử Khám Bệnh")] LichSuKhamBenh = 85,
            [Description("Lịch Sử Xác Nhận BHYT")] LichSuXacNhanBHYT = 86,
            [Description("Lịch Sử Thu Ngân")] LichSuThuNgan = 87,
            [Description("Lịch Sử Xuất Thuốc")] LichSuQuayThuoc = 88,
            [Description("Danh Mục Lý Do Tiếp Nhận")] DanhMucLyDoTiepNhan = 91,
            //[Description("Danh Mục Người Giới Thiệu")] DanhMucNguoiGioiThieu = 92,
            [Description("Danh Mục Nơi Giới Thiệu")] DanhMucNoiGioiThieu = 93,
            [Description("Phẫu thuật thủ thuật theo ngày")] PhauThuatThuThuatTheoNgay = 94,
            [Description("Lịch sử phẫu thuật thủ thuật")] LichSuPhauThuatThuThuat = 95,

            [Description("Danh Mục Giường Bệnh")] DanhMucGiuongBenh = 901,
            [Description("Tình Trạng Giường Bệnh")] TinhTrangGiuongBenh = 902,
            [Description("Danh Mục Dân Tộc")] DanToc = 102,

            #endregion NhomHeThong 
            [Description("Danh Mục Nhóm Dịch Vụ Bệnh Viện")] DanhMucNhomDichVuBenhVien = 96,
            [Description("Danh Mục Phương Pháp Vô Cảm")] DanhMucPhuongPhapVoCam = 97,
            [Description("Cận Lâm Sàng")] CanLamSang = 98,
            [Description("Danh Mục Quản Lý ICD")] QuanLyICD = 175,
            [Description("Danh Mục Công Ty Bảo Hiểm Tư Nhân")] DanhMucCongTyBhtn = 176,
            [Description("Danh Mục Công Ty Ưu Đãi")] DanhMucCongTyUuDai = 177,

            #region NhomPhongBan
            #endregion NhomPhongBan

            #region NhomKhoaPhong
            [Description("Danh Mục Khoa Phòng")] DanhMucKhoaPhong = 31,
            [Description("Danh Mục Khoa Phòng - Phòng Khám")] DanhMucKhoaPhongPhongKham = 32,
            [Description("Danh Mục Khoa Phòng - Nhân Viên")] DanhMucKhoaPhongNhanVien = 33,
            #endregion NhomKhoaPhong

            #region LichPhanCong
            [Description("Danh Mục Lịch Phân Công Ngoại Trú")] DanhMucLichPhanCongNgoaiTru = 34,
            #endregion

            #region NhomNhanVien
            [Description("Danh Mục Nhân Viên")]
            DanhMucNhanVien = 35,
            #endregion
            #endregion DanhMuc

            //[Description("Yêu Cầu Khám Bệnh")] YeuCauKhamBenh = 26,
            [Description("Danh Sách Chờ Khám")] DanhSachChoKham = 27,

            [Description("Yêu Cầu Tiếp Nhận")] YeuCauTiepNhan = 90,
            [Description("Yêu Cầu Tiếp Nhận Chỉnh Sửa Thông Tin Hành Chính")] YeuCauTiepNhanChinhSuaThongTinHanhChinh = 9001,


            [Description("Dược Phẩm Tồn Kho")] DuocPhamTonKho = 64,
            [Description("Dược Phẩm Sắp Hết Hạn")] DuocPhamSapHetHan = 65,
            [Description("Dược Phẩm Đã Hết Hạn")] DuocPhamDaHetHan = 66,
            [Description("Khám Bệnh")] KhamBenh = 67,
            [Description("Đối tượng ưu đãi dịch vụ kỹ thuật")] DoiTuongUuDaiDichVuKyThuat = 69,
            [Description("Đối tượng ưu đãi dịch vụ khám bệnh")] DoiTuongUuDaiDichVuKhamBenh = 89,
            [Description("Xác Nhận BHYT")] XacNhanBHYT = 70,
            [Description("Xác Nhận BHYT Đã Hoàn Thành")] XacNhanBhytDaHoanThanh = 105,
            [Description("Thu Ngân")] ThuNgan = 71,
            [Description("Nhà Thuốc")] QuayThuoc = 72,
            [Description("Lời Dặn")] LoiDan = 73,
            [Description("Toa Thuốc Mẫu")] ToaThuocMau = 74,
            [Description("Báo Cáo Chi Tiết Doanh Thu Theo Bác Sĩ")] BaoCaoChiTietDoanhThuTheoBacSi = 75,
            [Description("Báo Cáo Chi Tiết Doanh Thu Theo Khoa Phòng")] BaoCaoChiTietDoanhThuTheoKhoaPhong = 76,
            [Description("Báo Cáo Thu Viện Phí Người Bệnh")] BaoCaoThuVienPhiBenhNhan = 77,
            //[Description("Báo Cáo Nội Trú Ngoại Trú")] BaoCaoNoiTruNgoaiTru = 78,
            [Description("Báo Cáo Tổng Hợp Doanh Thu Theo Bác Sĩ")] BaoCaoTongHopDoanhThuTheoBacSi = 79,
            [Description("Báo Cáo Tổng Hợp Doanh Thu Theo Khoa Phòng")] BaoCaoTongHopDoanhThuTheoKhoaPhong = 80,
            //[Description("Theo Dõi Tình Hình Thanh Toán Viện Phí")] TheoDoiTinhHinhThanhToanVienPhi = 81,
            [Description("Báo Cáo Danh Sách Thu Tiền Viện Phí")] BaoCaoDanhSachThuTienVienPhi = 82,
            [Description("Gửi Bảo Hiểm Y Tế")] GuiBaoHiemYTe = 99,
            [Description("Danh Mục Quốc Gia")] DanhMucQuocGia = 101,
            [Description("Lịch Sử Cận Lâm Sàng")] LichSuCanLamSang = 103,
            [Description("Lịch Sử Gửi BHYT")] LichSuGuiBHYT = 104,
            //[Description("Xác Nhận Thu Ngân Đã Hoàn Thành")] XacNhanThuNganDaHoanThanh = 107,
            //[Description("Danh Sách Lịch Sử Bán Thuốc")] DanhSachLichSuBanThuoc = 108,
            [Description("Danh Mục Hợp Đồng Thầu Vật Tư")] DanhMucHopDongThauVatTu = 109,
            [Description("Danh Mục Định Mức Vật Tư Tồn Kho")] DanhMucDinhMucVatTuTonKho = 120,
            [Description("Danh Mục Dược Phẩm Bệnh Viện Phân Nhóm")] DanhMucDuocPhamBenhVienPhanNhom = 121,


            [Description("Nhập Kho Dược Phẩm")] NhapKhoDuocPham = 55,
            [Description("Xuất Kho Dược Phẩm")] XuatKhoDuocPham = 56,
            [Description("Duyệt Nhập Kho Dược Phẩm")] DuyetNhapKhoDuocPham = 147, //Kế toán duyệt nhập kho DP
            [Description("DS Yêu Cầu Lĩnh Dược Phẩm")] DanhSachYeuCauLinhDuocPham = 148, //DS yêu cầu lĩnh DP
            [Description("Tạo Yêu Cầu Lĩnh Thường Dược Phẩm")] TaoYeuCauLinhThuongDuocPham = 149, //Menu tạo phiếu lĩnh thường DP
            [Description("Tạo Yêu Cầu Lĩnh Bù Dược Phẩm")] TaoYeuCauLinhBuDuocPham = 150, //Menu tạo phiếu lĩnh bù DP
            [Description("Tạo Yêu Cầu Lĩnh Trực Tiếp Dược Phẩm")] TaoYeuCauLinhTrucTiepDuocPham = 151, //Menu tạo phiếu lĩnh trực tiếp DP
            [Description("Duyệt Yêu Cầu Lĩnh Dược Phẩm")] DuyetYeuCauLinhDuocPham = 152, //Duyệt yêu cầu lĩnh DP

            [Description("Nhập Kho Vật Tư")] NhapKhoVatTu = 9012,
            [Description("Duyệt Nhập Kho Vật Tư")] DuyetNhapKhoVatTu = 9011, //Kế toán duyệt nhập kho VT
            [Description("DS Yêu Cầu Lĩnh Vật Tư")] DanhSachYeuCauLinhVatTu = 153, //DS yêu cầu lĩnh VT
            [Description("Tạo Yêu Cầu Lĩnh Thường Vật Tư")] TaoYeuCauLinhThuongVatTu = 154, //Menu tạo phiếu lĩnh thường VT
            [Description("Tạo Yêu Cầu Lĩnh Bù Vật Tư")] TaoYeuCauLinhBuVatTu = 155, //Menu tạo phiếu lĩnh bù VT
            [Description("Tạo Yêu Cầu Lĩnh Trực Tiếp Vật Tư")] TaoYeuCauLinhTrucTiepVatTu = 156, //Menu tạo phiếu lĩnh trực tiếp VT
            [Description("Duyệt Yêu Cầu Lĩnh Vật Tư")] DuyetYeuCauLinhVatTu = 157, //Duyệt yêu cầu lĩnh VT

            //[Description("DS dược phẩm cần lĩnh trực tiếp")] DanhSachCanLinhTrucTiepDuocPham = 143,
            //[Description("DS vật tư cần lĩnh trực tiếp")] DanhSachCanLinhTrucTiepVatTu = 144,
            //[Description("DS dược phẩm cần bù")] DanhSachCanLinhBuDuocPham = 145,
            //[Description("DS vật tư cần bù")] DanhSachCanLinhBuVatTu = 146,
            [Description("Yêu Cầu Hoàn Trả Dược Phẩm")] YeuCauHoanTraDuocPham = 158,
            [Description("Yêu Cầu Hoàn Trả Vật Tư")] YeuCauHoanTraVatTu = 159,
            //[Description("Danh Sách Duyệt Yêu Cầu Hoàn Trả Dược Phẩm")] DanhSachDuyetYeuCauHoanTraDuocPham = 160,
            //[Description("Danh Sách Duyệt Yêu Cầu Hoàn Trả Vật Tư")] DanhSachDuyetYeuCauHoanTraVatTu = 161,
            [Description("Duyệt Yêu Cầu Hoàn Trả Dược Phẩm")] DuyetYeuCauHoanTraDuocPham = 162,
            [Description("Duyệt Yêu Cầu Hoàn Trả Vật Tư")] DuyetYeuCauHoanTraVatTu = 163,
            [Description("Vật Tư Tồn Kho")] VatTuTonKho = 164,
            [Description("Vật Tư Sắp Hết Hạn")] VatTuSapHetHan = 165,
            [Description("Vật Tư Đã Hết Hạn")] VatTuDaHetHan = 166,
            [Description("Khám bệnh đang khám")] KhamBenhDangKham = 167,
            [Description("Nhập Kho Marketing")] NhapKhoMarketing = 168,
            [Description("Xuất Kho Marketing")] XuatKhoMarketing = 169,
            [Description("Quà Tặng Marketing")] QuaTangMarketing = 170,

            [Description("Xuất Kho Vật Tư")] XuatKhoVatTu = 8000,

            [Description("Xuất Kho Dược Phẩm Khác")] XuatKhoDuocPhamKhac = 9020,
            [Description("Xuất Kho Vật Tư Khác")] XuatKhoVatTuKhac = 9030,
            [Description("Gói Dịch Vụ Marketing")] GoiDichVuMarketing = 171,
            [Description("Gói Dịch Vụ Nhóm Thường Dùng")] GoiDichVuNhomThuongDung = 172,
            [Description("Chương Trình Marketing Theo Gói Dịch Vụ")] GoiDvChuongTrinhMarketing = 173,
            [Description("Chương trình Marketing Voucher")] VoucherMarketing = 174,

            [Description("Danh Mục Marketing")] DanhSachMarketing = 9040,


            [Description("DS Lấu Mẫu XN")] LayMauXetNghiem = 178,
            [Description("DS Gởi Mẫu XN")] GoiMauXetNghiem = 179,
            [Description("DS Nhận Mẫu XN")] NhanMauXetNghiem = 180,
            [Description("DS Kết Quả XN")] KetQuaXetNghiem = 181,
            [Description("Gạch Nợ")] CongNoBhtn = 182,
            [Description("DS Yêu Cầu Dự Trù Mua Dược Phẩm")] DanhSachYeuCauDuTruMuaDuocPham = 183,
            [Description("DS Tổng Hợp Dự Trù Mua Dược Phẩm Tại Khoa")] DanhSachTongHopDuTruMuaDuocPhamTaiKhoa = 184,
            [Description("DS Tổng Hợp Dự Trù Mua Dược Phẩm Tại Khoa Dược")] DanhSachTongHopDuTruMuaDuocPhamTaiKhoaDuoc = 185,
            [Description("DS Tổng Hợp Dự Trù Mua Dược Phẩm Tại Giám Đốc")] DanhSachTongHopDuTruMuaDuocPhamTaiGiamDoc = 186,
            [Description("Xác Nhận Nhập Liệu Gạch Nợ")] CongNoBhtnXacNhanNhapLieu = 187,
            [Description("Kỳ Dự Trù")] KyDuTru = 188,

            [Description("Báo cáo công nợ công ty bảo hiểm tư nhân")] BaoCaoCongNoCongTyBhtn = 189,

            [Description("DS Yêu Cầu Dự Trù Mua Vật Tư")] DanhSachYeuCauDuTruMuaVatTu = 190,
            [Description("DS Tổng Hợp Dự Trù Mua Vật Tư Tại Khoa")] DanhSachTongHopDuTruMuaVatTuTaiKhoa = 191,
            [Description("DS Tổng Hợp Dự Trù Mua Vật Tư Tại Khoa Dược")] DanhSachTongHopDuTruMuaVatTuTaiKhoaDuoc = 192,
            [Description("DS Tổng Hợp Dự Trù Mua Vật Tư Tại Giám Đốc")] DanhSachTongHopDuTruMuaVatTuTaiGiamDoc = 193,
            [Description("DS Duyệt Kết Quả XN")] DuyetKetQuaXetNghiem = 194,
            [Description("DS Duyệt Yêu Cầu Chạy Lại XN")] DuyetYeuCauChayLaiXetNghiem = 195,
            [Description("DS Chỉ Số XN")] ChiSoXetNghiem = 196,
            [Description("Thiết Bị Xét Nghiệm")] ThietBiXetNghiem = 197,
            [Description("Tiếp Nhận Nội Trú")] TiepNhanNoiTru = 198,
            [Description("Tổng hợp y lệnh")] TongHopYLenh = 199,
            //            [Description("Kết quả siêu âm")] KetQuaSieuAm = 200,
            //            [Description("Kết quả X-Quang")] KetQuaXQuang = 201,
            //            [Description("Kết quả nội soi")] KetQuaNoiSoi = 202,
            //            [Description("Kết quả điện tim")] KetQuaDienTim = 203,

            [Description("Nhập Kho Máu")] NhapKhoMau = 204,
            [Description("Duyệt Nhập Kho Máu")] DuyetNhapKhoMau = 205,
            [Description("Lưu Trữ Hồ Sơ")] LuuTruHoSo = 206,
            [Description("Trả Thuốc Từ Người Bệnh")] TraThuocTuBenhNhan = 207,
            [Description("Duyệt Hoàn Trả Thuốc Từ Người Bệnh")] DuyetTraThuocTuBenhNhan = 208,
            [Description("Duyệt Hoàn Trả Vật Tư Từ Người Bệnh")] DuyetTraVatTuTuBenhNhan = 209,
            [Description("Trả Vật Tư Từ Người Bệnh")] TraVatTuTuBenhNhan = 215,
            [Description("Xác Nhận BHYT Nội Trú")] XacNhanBhytNoiTru = 210,
            [Description("Công Nợ Người Bệnh")] CongNoBenhNhan = 211,

            [Description("Báo Cáo Lưu Trữ Hồ Sơ Bệnh An")] BaoCaoLuuTruHoSoBenhAn = 212,
            [Description("Báo Cáo Bác Sĩ Danh Sách Khám ngoại trú")] BaoCaoBSDanhSachKhamNgoaiTru = 213,
            [Description("Báo Cáo Xuất Nhập Tồn")] BaoCaoXuatNhapTon = 230,
            [Description("Báo Cáo Tiếp Nhận Người Bệnh Khám")] BaoCaoTiepNhanBenhNhanKham = 231,

            [Description("Báo Cáo Kết Quả Khám Chữa Bệnh")] BaoCaoKetQuaKhamChuaBenh = 216,
            [Description("Báo Cáo Viện Phí Thu Tiền")] BaoCaoVienPhiThuTien = 223,
            [Description("Báo Cáo Thống Kê Đơn Thuốc")] BaoCaoThongKeDonThuoc = 222,
            [Description("Báo Cáo Doanh Thu Nhà Thuốc")] BaoCaoDoanhThuNhaThuoc = 214,
            [Description("Báo Cáo Hoạt Động Khoa Khám Bệnh")] BaoCaoHoatDongKhoaKhamBenh = 218,
            [Description("Báo Cáo Thực Hiện Cận Lâm Sàng")] BaoCaoThucHienCls = 219,
            [Description("Danh Sách Người Bệnh Phẫu Thuật")] DanhSachBenhNhanPhauThuat = 220,
            [Description("Báo Cáo Lưu Kết Quả Xét Nghiệm Hàng Ngày")] BaoCaoLuuKetQuaXetNghiemHangNgay = 251,
            #region Điều trị nội trú
            [Description("DS Điều Trị Nội Trú")] DanhSachDieuTriNoiTru = 9050,
            #endregion Điều trị nội trú
            [Description("Danh Mục Chế Độ Ăn")] DanhMucCheDoAn = 224,

            #region Khám đoàn
            [Description("Khám Đoàn Công Ty")] KhamDoanCongTy = 232,
            [Description("Khám Đoàn Chỉ Số Sinh Tồn")] KhamDoanChiSoSinhTon = 233,
            [Description("Khám Đoàn Hợp Đồng Khám")] KhamDoanHopDongKham = 234,
            [Description("Lịch Sử Tiếp Nhận Khám Sức Khỏe")] KhamDoanLichSuTiepNhanKhamSucKhoe = 235,
            [Description("Kết Luận Cận Lâm Sàng Khám Sức Khỏe Đoàn")] KhamDoanKetLuanCanLamSangKhamSucKhoeDoan = 236,
            [Description("Khám Đoàn Tiếp Nhận")] KhamDoanTiepNhan = 237,
            [Description("Khám Đoàn Gói Khám Sức Khỏe")] KhamDoanGoiKhamSucKhoe = 238,
            [Description("Khám Đoàn Yêu Cầu Nhân Sự Khám Sức Khỏe")] KhamDoanYeuCauNhanSuKhamSucKhoe = 239,
            [Description("Kết Luận Khám Sức Khỏe Đoàn")] KhamDoanKetLuanKhamSucKhoeDoan = 240,
            [Description("Khám Đoàn Duyệt Yêu Cầu Nhân Sự Khám Sức Khỏe Khth")] KhamDoanDuyetYeuCauNhanSuKhamSucKhoeKhth = 241,
            [Description("Khám Đoàn Duyệt Yêu Cầu Nhân Sự Khám Sức Khỏe Phòng Nhân Sự")] KhamDoanDuyetYeuCauNhanSuKhamSucKhoePhongNhanSu = 242,
            [Description("Khám Đoàn Duyệt Yêu Cầu Nhân Sự Khám Sức Khỏe Giám Đốc")] KhamDoanDuyetYeuCauNhanSuKhamSucKhoeGiamDoc = 243,
            [Description("Khám Đoàn Khám Bệnh")] KhamDoanKhamBenh = 244,
            [Description("Khám Đoàn Khám Bệnh Tất Cả Phòng")] KhamDoanKhamBenhTatCaPhong = 291,
            [Description("Dự Trù Số Lượng Người Khám Sức Khỏe Thực Hiện Dịch Vụ LS - CLS ")] KhamDoanThongKeSoNguoiKhamSucKhoeLSCLS = 329,
            #endregion

            [Description("Cập Nhật Dược Phẩm Tồn Kho")] CapNhatDuocPhamTonKho = 245,
            [Description("Cập Nhật Vật Tư Tồn Kho")] CapNhatVatTuTonKho = 246,
            [Description("Báo Cáo Bảng Kê Chi Tiết TTCN")] BaoCaoBangKeChiTietTTCN = 247,
            [Description("Báo Cáo Thẻ Kho")] BaoCaoTheKho = 248,
            [Description("Báo Cáo Tồn Kho")] BaoCaoTonKho = 249,

            [Description("Báo Cáo Danh Thu Theo Nhóm Dịch Vụ")] BaoCaoDoanhThuTheoNhomDichVu = 250,
            [Description("Báo Cáo Xuất Nhập Tồn Vật Tư")] BaoCaoXNTVatTu = 252,
            [Description("Số Xét Nghiệm Sàng Lọc Hiv")] BaoCaoSoXetNghiemSangLocHiv = 253,
            [Description("Tổng Hợp Số Lượng Xét Nghiệm")] BaoCaoTongHopSoLuongXetNghiemTheo = 254,
            [Description("Bệnh Nhân Làm Xét Nghiệm")] BaoCaoBenhNhanLamXetNghiem = 255,
            [Description("Điều Chuyển Kho Nội Bộ Dược Phẩm")] DanhSachDieuChuyenNoiBoDuocPham = 256,
            [Description("Duyệt Điều Chuyển Kho Nội Bộ Dược Phẩm")] DanhSachDuyetDieuChuyenNoiBoDuocPham = 257,

            [Description("Báo Cáo Người Bệnh Khám Ngoại Trú")] BaoCaoBenhNhanKhamNgoaiTru = 258,
            [Description("Báo Cáo Số Lượng Thủ Thuật")] BaoCaoSoLuongThuThuat = 259,
            [Description("Báo Cáo Sổ Phúc Trình Phẫu Thuật/Thủ Thuật")] BaoCaoSoPhucTrinhPhauThuatThuThuat = 260,
            [Description("Báo Cáo Hoạt Động CLS Theo Khoa")] BaoCaoHoatDongClsTheoKhoa = 261,
            [Description("Báo Cáo Sổ Thống Kê CLS")] BaoCaoSoThongKeCls = 262,
            [Description("Tạo Bệnh Án Sơ Sinh")] TaoBenhAnSoSinh = 263,
            [Description("Từ Điển Dịch Vụ Kỹ Thuật")] TuDienDichVuKyThuat = 264,
            [Description("Tiêm Chủng Khám Sàng Lọc")] TiemChungKhamSangLoc = 265,
            [Description("Tiêm Chủng Thực Hiện Tiêm")] TiemChungThucHienTiem = 266,
            [Description("Tiêm Chủng Lịch Sử Tiêm")] TiemChungLichSuTiem = 267,
            [Description("Báo Cáo Dịch Vụ Trong Gói Khám Đoàn")] BaoCaoDichVuTrongGoiKhamDoan = 280,
            [Description("Báo Cáo Dịch Vụ Ngoài Gói Khám Đoàn")] BaoCaoDichVuNgoaiGoiKhamDoan = 281,

            [Description("Báo Cáo Tổng Hợp Kết Quả Khám Sức Khỏe")] BaoCaoTongHopKetQuaKSK = 277,
            [Description("Báo Cáo Hiệu Quả Công Việc ")] BaoCaoHieuQuaCongViec = 271,
            [Description("Báo Cáo Tiếp Nhận Bệnh Phẩm")] BaoCaoTiepNhanBenhPham = 272,
            [Description("Báo Cáo Tồn Kho")] BaoCaoTonKhoXN = 273,
            [Description("Báo Cáo Tồn Kho")] BaoCaoTonKhoKT = 275,
            [Description("Báo Cáo Tình Hình Trả Nhà Cung Cấp")] BaoCaoTinhHinhTraNCC = 274,
            [Description("Báo Cáo Tình Hình Nhập Nhà Cung Cấp Chi Tiết")] BaoCaoTinhHinhNhapNCCChiTiet = 276,
            [Description("Báo Cáo Dược Chi Tiết Xuất Nội Bộ")] BaoCaoDuocChiTietXuatNoiBo = 282,
            [Description("Báo Cáo Chi Tiết Miễn Phí Trốn Viện")] BaoCaoChiTietMienPhiTronVien = 284,
            [Description("Báo Cáo Biên Bản Kiểm Kê Kế Toán")] BaoCaoBienBanKiemKeKT = 278,
            [Description("Báo Cáo Bảng Kê Phiếu Xuất Kho")] BaoCaoBangKePhieuXuatKho = 279,
            [Description("Báo Cáo Tình Hình Nhập Từ Nhà Cung Cấp")] BaoCaoTinhHinhNhapTuNhaCungCap = 283,
            [Description("Báo Cáo Tổng Hợp Doanh Thu Thai Sản Đã Sinh")] BaoCaoTongHopDoanhThuThaiSanDaSinh = 285,
            [Description("Báo Cáo Tổng Hợp Đăng Ký Gói Dịch Vụ")] BaoCaoTongHopDangKyGoiDichVu = 286,
            [Description("Báo Cáo Sổ Chi Tiết Vật Tư Hàng Hóa")] BaoCaoSoChiTietVatTuHangHoa = 287,
            [Description("Báo Cáo Doanh Thu Khám Đoàn Theo Nhóm Dịch Vụ")] BaoCaoDoanhThuKhamDoanTheoNhomDV = 288,
            [Description("Báo Cáo Bảng Kê Xuất Thuốc Theo Bệnh Nhân")] BaoCaoBangKeXuatThuocTheoBenhNhan = 289,
            [Description("Báo Cáo Hoạt Động CLS")] BaoCaoHoatDongCls = 290,
            [Description("Báo Cáo Thuốc Sắp Hết Hạn Dùng")] BaoCaoThuocSapHetHanDung = 292,
            [Description("Báo cáo dược tình hình xuất nội bộ")] BaoCaoDuocTinhHinhXuatNoiBo = 268,
            [Description("Báo Cáo Kế Toán Nhập Xuất Tồn Chi Tiết")] BaoCaoKTNhapXuatTonChiTiet = 269,
            [Description("Báo Cáo Số Liệu Tính Thời Gian Sử Dụng DV Của Khách Hàng")] BaoCaoSoLieuThoiGianSuDungDV = 293,
            [Description("Báo Cáo Doanh Thu Khám Đoàn Theo Khoa Phòng")] BaoCaoDoanhThuKhamDoanTheoKP = 294,
            [Description("Báo Cáo Chi Tiết Hoa Hồng Của Người Giới Thiệu")] BaoCaoChiTietHoaHongCuaNguoiGT = 295,
            [Description("Báo Cáo Cam Kết Tự Nguyện Sử Dụng Thuốc - DV Ngoài BHYT")] BaoCaoCamKetSuDungThuocNgoaiBHYT = 296,
            [Description("Báo Cáo Bảng Kê Giao Hóa Đơn Sang Phòng Kế Toán")] BaoCaoBangKeGiaoHoaDonSangPKT = 297,
            [Description("Báo Cáo Hoạt Động Nội Trú")] BaoCaoHoatDongNoiTru = 298,
            [Description("Báo Cáo Hoạt Động Khám Đoàn")] BaoCaoHoatDongKhamDoan = 300,
            [Description("Báo Cáo Biên Bản Kiểm Kê Dược VT")] BaoCaoBienBanKiemKeDuocVT = 299,
            [Description("Báo Cáo Thống kê khám sức khỏe")] BaoCaoThongKeKSK = 320,


            #region lịch sử khám chữa bệnh
            [Description("Lịch sử khám chữa bệnh")] LichSuKhamChuaBenh = 316,
            [Description("Lịch sử khám chữa bệnh khám bệnh")] LichSuKhamChuaBenhKhamBenh = 317,
            [Description("Lịch sử khám chữa bệnh CLS")] LichSuKhamChuaBenhCanLamSang = 318,
            [Description("Lịch sử khám chữa bệnh y lệnh")] LichSuKhamChuaBenhYLenh = 319,
            #endregion
            [Description("Báo Cáo Người Bệnh Đến Khám")] BaoCaoNguoiBenhDenKham = 321,
            [Description("Báo Cáo Người Bệnh Đến Làm DVKT")] BaoCaoNguoiBenhDenLamDVKT = 322,
            [Description("Báo Cáo Tra cứu dữ liệu")] BaoCaoTraCuuDuLieu = 323,
            [Description("Báo Cáo Tổng Hợp Công Nợ Chưa Thanh Toán")] BaoCaoTongHopCongNoChuaThanhToan = 324,
            [Description("Báo Cáo Bảng Kê Chi Tiết Theo Người Bệnh")] BaoCaoBangKeChiTietTheoNguoiBenh = 325,
            [Description("Báo Cáo Khám Sức Khỏe Chuyên Khoa")] BaoCaoKSKChuyenKhoa = 330,
            [Description("Báo Cáo Hoạt Động Khám Bệnh Theo Dịch Vụ")] BaoCaoHoatDongKhamBenhTheoDichVu = 331,
            [Description("Báo Cáo Hoạt Động Khám Bệnh Theo Khoa Phòng")] BaoCaoHoatDongKhamBenhTheoKhoaPhong = 332,
            [Description("Báo Cáo Thu Viện Phí Chưa Hoàn")] BaoCaoThuVienPhiChuaHoan = 333,
            [Description("Bảng Kê Thuốc Và Vật Tư Phẫu Thuật")] BangKeThuocVatTuPhauThuat = 326,
            [Description("Báo Cáo Tồn Kho Vật Tư Y Tế")] BaoCaoTonKhoVatTuYTe = 327,
            [Description("Báo Cáo Thẻ Kho Vật Tư Y Tế")] BaoCaoTheKhoVatTuYTe = 328,

            [Description("Báo cáo  dịch vụ phát sinh ngoài gói của kế toán")] BaoCaoDichVuPhatSinhNgoaiGoiCuaKeToan = 335,
            [Description("Bảng thống kê tiếp nhận nội trú,ngoại trú")] BangThongKeTiepNhanNoiTruVaNgoaiTru = 336,
            [Description("Báo Cáo Dịch Vụ Chiếu Tia PLASMA Lạnh Hỗ Trợ Điều Trị Vết Thương")] BaoCaoDichVuTiaPlasMaHoTroDieuTriVetThuong = 337,
            [Description("Báo Cáo Doanh Thu Khám Đoàn Theo Nhóm Dịch Vụ")] BaoCaoDoanhThuKhamDoanTheoNhomDichVu = 338,
            [Description("Báo Cáo Bệnh Nhân Ra Viện Nội Trú")] BaoCaoBenhNhanRaVienNoiTru = 339,

            [Description("Thống Kê thuốc theo bác sĩ")] ThongKeThuocTheoBacSi = 340,
            [Description("Thống Kê BS Kê Đơn Theo Thuốc")] ThongKeBSKeDonTheoThuoc = 341,
            [Description("Thống Kê Các Dịch Vụ Chưa Lấy Lên Biên Lai Thu Tiền")] ThongKeCacDichVuChuaLayLenBienLaiThuTien = 342,
            [Description("Danh Sách BA Ra Viện Chưa Xác Nhận Hoàn Tất Chi Phí")] DanhSachBARaVienChuaXacNhanHoanTatChiPhi = 343,
            [Description("Danh Sách Thu Viện Phí Nội Trú Và Ngoại Trú Chưa Hoàn")] DanhSachThuVienPhiNoiTruVaNgoaiTruChuaHoan = 344,
            [Description("Danh Mục Dịch Vụ Kỹ Thuật Bệnh Viện")] DanhMucDichVuKyThuatBenhVien = 346,
            [Description("Danh Mục Dịch Vụ Khám Bệnh Bệnh Viện")] DanhMucDichVuKhamBenhBenhVien = 347,

            [Description("Danh Sách Xuất Chứng Từ Excel")] DanhSachXuatChungTuExcel = 345,
            [Description("Danh Sách Lịch Sử Hủy Bán Thuốc")] DanhSachLichSuHuyBanThuoc = 348,
            [Description("Báo cáo kết quả khám chữa bệnh KT")] BaoCaoKetQuaKhamChuaBenhKT = 349,
            //[Description("Danh Sách Gáy Bệnh Án")] DanhSachGayBenhAn = 355
            [Description("Báo cáo Hoạt Động Nội Trú Chi Tiết")] BaoCaoHoatDongNoiTruChiTiet = 356,
            [Description("Báo cáo Tình Hình Bệnh Tật Tử Vong")] BaoCaoTinhHinhBenhTatTuVong = 357,
            [Description("Danh Mục Bệnh Và Nhóm Bệnh ")] DanhMucBenhVaNhomBenh = 358,

            #region Bệnh án điện tử
            [Description("Danh Mục Gáy Bệnh Án")] DanhMucGayBenhAn = 334,
            [Description("Bệnh Án Điện Tử")] BenhAnDienTu = 351,

            #endregion

            [Description("Danh sách xác nhận nội trú và ngoại trú BHYT")] DSXNNgoaiTruVaNoiTruBHYT = 359,
            [Description("Báo cáo Nhập Xuất Tồn")] BaoCaoNhapXuatTon = 360,

            [Description("Danh sách đơn thuốc chờ cấp thuốc BHYT")] DanhSachDonThuocChoCapThuocBHYT = 361,
            [Description("Lịch sử xuất thuốc cấp thuốc BHYT")] LichSuXuatThuocCapThuocBHYT = 362,
            [Description("Mở Lại bệnh án")] MoLaiBenhAn = 363,

            [Description("Nhập vật tư thuộc nhóm KSNK")] NhapVatTuThuocNhomKSNK = 364,
            [Description("Xuất Kho Vật Tư thuộc nhóm KSNK")] XuatKhoVatTuThuocNhomKSNK = 365,
            [Description("Xuất Kho Khác Vật Tư thuộc nhóm KSNK")] XuatKhoKhacVatTuThuocNhomKSNK = 366,

            [Description("Yêu cầu dự trù mua nhóm KSNK")] YeuCauDuTruMuaNhomKSNK = 367,
            [Description("THDT mua tại KSNK")] THDTMuaTaiKSNK = 368,
            [Description("THDT mua tại hành chính")] THDTMuaTaiHanhChinh = 369,
            [Description("THDT mua tại giám đốc")] THDTMuaTaiGiamDoc = 370,

            [Description("Yêu Cầu Hoàn Trả KSNK")] YeuCauHoanTraKSNK = 371,
            [Description("Duyệt Yêu Cầu Hoàn Trả KSNK")] DuyetYeuCauHoanTraKSNK = 372,

            [Description("DS Yêu Cầu Lĩnh KSNK")] DanhSachYeuCauLinhKSNK = 373, //DS yêu cầu lĩnh KSNK
            [Description("Tạo Yêu Cầu Lĩnh Thường KSNK")] TaoYeuCauLinhThuongKSNK = 374, //Menu tạo phiếu lĩnh thường KSNK
            [Description("Tạo Yêu Cầu Lĩnh Bù KSNK")] TaoYeuCauLinhBuKSNK = 375, //Menu tạo phiếu lĩnh bù KSNK
            [Description("Duyệt Yêu Cầu Lĩnh KSNK")] DuyetYeuCauLinhKSNK = 376, //Duyệt yêu cầu lĩnh KSNK

            [Description("Báo Cáo Xét Nghiệm Xuất Nhập Tồn Kho Xét Nghiệm")] BaoCaoXNXuatNhapTonKhoXetNghiem = 377,
            [Description("Báo Cáo Xét Nghiệm Phiếu Nhập Hoá Chất")] BaoCaoXNPhieuNhapHoaChat = 378,
            [Description("Báo Cáo Xét Nghiệm Phiếu Xuất Hóa Chất")] BaoCaoXNPhieuXuatHoaChat = 379,

            [Description("Danh Sách Đề Nghị Thanh Toán Chi Phí KCB Ngoại Trú")] DSThanhToanChiPhiKCBNgoaiTru = 380,
            [Description("Danh Sách Đề Nghị Thanh Toán Chi Phí KCB Nội Trú")] DSThanhToanChiPhiKCBNoiTru = 381,

            [Description("Quản Lý Ngày Lễ")] QuanLyNgayLe = 382,
            [Description("Quản Lý Lịch Làm Việc")] QuanLyLichLamViec = 383,

            [Description("Danh Mục Cấu Hình Thuê Phòng")] DanhMucCauHinhThuePhong = 384,
            [Description("Danh Mục Lịch Sử Thuê Phòng")] DanhMucLichSuThuePhong = 385,
            [Description("Thống Kê Dịch Vụ Khám Sức Khỏe")] ThongKeDichVuKhamSucKhoe = 386,
            [Description("Danh Mục Quản Lý HDPP")] DanhMucQuanLyHDPP = 387,

            [Description("Dùng Để Đẩy Lên Cổng Giám Định BHYT 7980a")] DayLenCongGiamDinh7980a = 388,
            [Description("Giám Định BHYT 7980a Xuất Cho Kế Toán")] GiamDinhBHYT7980aXuatChoKeToan = 389,
            [Description("Cấu Hình Người Duyệt Theo Nhóm Dịch Vụ")] CauHinhNguoiDuyetTheoNhomDichVu = 390,

            [Description("Báo cáo tình hình nhập nhà cung cấp chi tiết")] BaoCaoTinhHinhNhapNhaCungCapChiTiet = 391,
            [Description("Báo cáo tình hình trả nhà cung cấp chi tiết")] BaoCaoTinhHinhTraNhaCungCapChiTiet = 392,

            [Description("Danh Mục Loại Giá")] DanhMucLoaiGiaDichVu = 393,

            [Description("VTYT Tình hình trả NCC")] VTYTTinhHinhTraNCC = 394,
            [Description("Báo Cáo Kế Toán Bảng Kê Chi Tiết Người Bệnh")] BaoCaoKeToanBangKeChiTietNguoiBenh = 395,
            [Description("VTYT Báo cáo chi tiết xuất nội bộ")] VTYTBaoCaoChiTietXuatNoiBo = 396,
            [Description("KHTH Báo Cáo Thống Kê SL Thủ Thuật")] KHTHBaoCaoThongKeSLThuThuat = 397,
            [Description("VTYT Báo cáo chi tiết hoàn trả nội bộ")] VTYTBaoCaoChiTietHoanTraNoiBo = 398,

            [Description("Báo cáo doanh thu khám đoàn theo nhóm dịch vụ lấy theo đơn giá thực tế")] BCDTKhamDoanTheoNhomDVDGThucTe = 399,
            [Description("Báo cáo doanh thu khám đoàn theo khoa phòng lấy theo giá thực tế")] BCDTKhamDoanTheoKhoaPhongDGThucTe = 400,
            [Description("Báo cáo doanh thu chia theo phòng")] BCDTChiaTheoPhong = 401,
            [Description("Báo cáo tổng hợp doanh thu theo nguồn bệnh nhân")] BCDTTongHopDoanhThuTheoNguonBenhNhan = 402,

            [Description("Danh Mục Cấu Hình Hệ Số Theo Nơi Giới Thiệu/ Hoa Hồng")] DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong = 403,
            [Description("Danh Mục Cấu Hình Đơn Vị Hành Chính")] DanhMucDonViHanhChinh = 404,
            #endregion Internal
        }

        public enum SecurityOperation
        {
            None = 0,
            View = 1,
            Process = 2,
            Add = 3,
            Delete = 4,
            Update = 5,
        }

        public enum Region
        {
            [Description("Tất cả")]
            All = 0,
            [Description("Bên trong")]
            Internal = 1,
            [Description("Bên ngoài")]
            External = 2,
        }


        public enum LanguageType
        {
            [Description("Việt Nam")]
            VietNam = 1,
            [Description("English")]
            English = 2
        }
        public enum AreaCode
        {
            [Description("+84")]
            VietNam = 1
        }
        public enum DataType
        {
            [Description("bool")]
            Boolean = 1,
            [Description("int")]
            Integer = 2,
            [Description("string")]
            String = 3,
            [Description("double")]
            Double = 4,
            [Description("date")]
            Date = 5,
            [Description("time")]
            Time = 6,
            [Description("datetime")]
            Datetime = 7,
            [Description("phone")]
            Phone = 8,
            [Description("email")]
            Email = 9,
            [Description("list")]
            List = 10
        }
        public enum TemplateType
        {
            [Description("Nội dung mẫu PDF")]
            NoiDungMauPDF = 1
        }

        public enum UserType
        {
            [Description("Nhân viên")]
            NhanVien = 1,
            [Description("Khách vãng lai")]
            KhachVangLai = 2,
        }

        public enum DefaultRole
        {
            [Description("Admin")]
            Admin = 1
        }
    }
}



