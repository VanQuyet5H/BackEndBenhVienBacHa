using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiHoSoDieuTriNoiTru
        {
            [Description("Phiếu sàng lọc dinh dưỡng")]
            PhieuSangLocDinhDuong = 1,
            [Description("Phiếu khai thác tiền sử dị ứng")]
            PhieuKhaiThacTienSuDiUng = 2,
            [Description("Trích biên bản hội chẩn")]
            TrichBienBanHoiChan = 3,
            [Description("Phiếu theo dõi chức năng sống")]
            PhieuTheoDoiChucNangSong = 4,
            [Description("Phiếu sơ kết 15 ngày điều trị")]
            PhieuSoKet15NgayDieuTri = 5,
            [Description("Phiếu tóm tắt thông tin điều trị và các dịch vụ")]
            PhieuTomTatThongTinDieuTri = 6,
            [Description("Biên bản hội chẩn phẫu thuật")]
            BienBanHoiChanPhauThuat = 7,
            [Description("Biên bản cam kết đồng ý gây mê/ gây tê")]
            BienBanCamKetGayMeGayTe = 9,
            [Description("Biên bản cam kết thực hiện phẫu thuật, thủ thuật")]
            BienBanCamKetPhauThuat = 8,
            [Description("Bảng kiểm an toàn phẫu thuật")]
            BangKiemAnToanPhauThuat = 10,
            [Description("Bảng kiểm an toàn người bệnh trước phẫu thuật")]
            BangKiemAnToanNguoiBenhPhauThuat = 11,
            [Description("Phiếu khám gây mê trước mổ")]
            PhieuKhamGayMeTruocMo = 12,
            [Description("Giấy tự nguyện triệt sản")]
            GiayTuNguyenTrietSan = 13,
            [Description("Giấy cam kết tự nguyện thực hiện kỹ thuật mới trong điều trị gây mê hồi sức theo yêu cầu")]
            GiayCamKetKyThuatMoi = 14,
            [Description("Giấy khám chữa bệnh theo yêu cầu")]
            GiayKhamChuaBenhTheoYc = 15,
            [Description("Bảng theo dõi hồi tỉnh")]
            BangTheoDoiHoiTinh = 16,
            [Description("Biên bản cam kết gây tê giảm đau trong đẻ - sau mổ")]
            BienBanCamKetGayTeGiamDauTrongDeSauMo = 17,
            [Description("Bảng kiểm bàn giao và nhận NB sau PT")]
            BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri = 18,
            [Description("Bảng theo dõi gây mê hồi sức")]
            BangTheoDoiGayMeHoiSuc = 19,
            [Description("Phiếu Công khai thuốc")]
            PhieuCongKhaiThuoc = 20,
            [Description("Phiếu Công khai Vật tư")]
            PhieuCongKhaiVatTu = 21,
            [Description("Phiếu đề nghị test trước khi dùng thuốc")]
            PhieuDeNghiTestTruocKhiDungThuoc = 22,
            [Description("Phiếu theo dõi truyền dịch")]
            PhieuTheoDoiTruyenDich = 23,
            [Description("Giấy chuyển tuyến")]
            GiayChuyenTuyen = 24,
            [Description("Bản kiểm trước tiêm chủng trẻ em")]
            BanKiemTruocTiemChungTreEm = 25,
            [Description("Phiếu chăm sóc")]
            PhieuChamSoc = 26,
            [Description("Phiếu theo dõi truyền máu")]
            PhieuTheoDoiTruyenMau = 27,
            [Description("Giấy ra viện")]
            GiayRaVien = 28,
            //[Description("Biên bản hội chẩn gây mê")] => hiện tại chưa ai làm
            //BienBanHoiChanGayMe = 29,
            [Description("Tóm tắt hồ sơ bệnh án")]
            TomTatHoSoBenhAn = 30,
            [Description("Giấy chứng sinh")]
            GiayChungSinh = 31,
            [Description("Giấy cam kết tự nguyện sử dụng thuốc")]
            GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBenhVien = 32,
            [Description("Giấy chứng nhận nghỉ việc hưởng bảo hiểm xã hội")]
            GiayChungNhanNghiViecHuongBHXH = 33,
            [Description("Giấy chứng sinh mang thai hộ")]
            GiayChungSinhMangThaiHo = 34,
            [Description("Biểu đồ chuyển dạ")]
            BieuDoChuyenDa = 35,
            [Description("Phiếu Công khai Dịch Vụ Kỹ Thuật")]
            PhieuCongKhaiDichVuKyThuat = 36,
            [Description("Phiếu Công khai Thuốc , vật tư y tế ")]
            PhieuCongKhaiThuocVatTu = 37,
            [Description("Hồ sơ chăm sóc điều dưỡng - hộ sinh ")]
            HoSoChamSocDieuDuongHoSinh = 39,
            [Description("Biên bản hội chẩn phẩu thuật sử dụng thuốc có dấu (*) ")]
            BienBanHoiChanPhauThuatSuDungThuocCoDau = 40,
            [Description("Giấy nghỉ dưỡng thai")]
            GiayNghiDuongThai = 41,
            [Description("Giấy chứng nhận phẫu thuật")]
            GiayChungNhanPhauThuat = 42,
            [Description("Giấy thoả thuận lựa chọn dịch vụ khám chữa bệnh theo yêu cầu")]
            GiayThoaThuanLuaChonDichVuKhamChuabenhTheoYeuCau = 43,
            [Description("Giấy cam kết tự nguyện sử dụng kỹ thuật mới trong điều trị gây mê hồi sức")]
            GiayCamKetKyThuatMoiHS = 44,
            [Description("Giấy phản ứng thuốc")]
            GiayPhanUngThuoc = 45,
            [Description("Giấy cam kết tự sử dụng thuốc ngoài bảo hiểm y tế")]
            GiayCamKetSuDungThuocNgoaiBHYT = 47,
            [Description("Giấy cam kết gây tê giảm đau trong đẻ - sau phẩu thuật")]
            GiayCamKetGayMeGiamDauTrongDeSauPhauThuat = 49,
        }
        public enum CheDoChamSoc
        {
            [Description("Chăm sóc cấp I")]
            ChamSocCapI = 1,
            [Description("Chăm sóc cấp II")]
            ChamSocCapII = 2,
            [Description("Chăm sóc cấp III")]
            ChamSocCapIII = 3
        }
        public enum DoiTuongSuDung
        {
            [Description("Người Bệnh")]
            BenhNhan = 1,
            [Description("Người Nhà")]
            NguoiNha = 2
        }

        public enum DonViTocDoTruyen
        {
            [Description("giọt/ph")]
            GiotTrenPhut = 1,
            [Description("ml/h")]
            MlTrenGio = 2
        }

        #region Phiếu sàng lọc dinh dưỡng
        public enum GiamCan
        {
            [Description("Không")]
            Khong = 0,
            [Description("Không rõ/chắc chắn giảm")]
            KhongRo = 1
        }

        public enum SoKgGiam
        {
            [Description("1-5kg")]
            MotDenNamKg = 0,
            [Description("6-10kg")]
            SauDenMuoiKg = 1,
            [Description("11-15kg")]
            MuoiMotDenMuoiLamKg = 2,
            [Description("15kg trở lên")]
            MuoiLamKgTroLen = 3,
            [Description("Không rõ/chắc chắn giảm")]
            KhongRoSoKgGiam = 4
        }

        public enum AnUongKem
        {
            [Description("Không")]
            Khong = 0,
            [Description("Có")]
            Co = 1
        }

        public enum TinhTrangBenhLyNang
        {
            [Description("Không")]
            Khong = 0,
            [Description("NB phẫu thuật đặc biệt")]
            PhauThuatDacBiet = 1,
            [Description("NB ghép tạng, viêm phúc mạc")]
            GhepTangViemPhucMac = 2,
            [Description("NB phù, báng bụng, viêm tuỵ cấp")]
            PhuBangBungViemTuyCap = 3,
            [Description("NB ăn giảm dưới 50% kéo dài trên 1 tuần")]
            GiamAn = 4
        }

        public enum KeHoachDinhDuong
        {
            [Description("Đường miệng")]
            DuongMieng = 0,
            [Description("Qua ống thông")]
            QuaOngThong = 1,
            [Description("Qua tĩnh mạch")]
            QuaTinhMach = 2
        }

        public enum TocDoTangCan
        {
            [Description("Tăng cân theo khuyến nghị")]
            TheoKhuyenNghi = 0,
            [Description("Tăng cân trên hoặc dưới mức khuyến nghị")]
            TrenDuoiMucKhuyenNghi = 1
        }

        public enum BenhKemTheo
        {
            [Description("Không")]
            Khong = 0,
            [Description("Tăng huyết áp, đái tháo đường, nghén nặng...")]
            TangHuyetAp = 1
        }
        #endregion


        public enum DoiTuongTiepNhan
        {
            [Description("Dịch vụ")]
            DichVu = 1,
            [Description("Bảo hiểm")]
            BaoHiem = 2
        }

        public enum SutCan1ThangQua
        {
            [Description("0 - 2 kg")]
            KhongDen2Kg = 0,
            [Description("2 - 5 kg")]
            HaiDen5Kg = 1,
            [Description("5 - 10 kg")]
            NamDen10Kg = 2
        }

        public enum AnKemLonHon5Ngay
        {
            [Description("Không")]
            Khong = 0,
            [Description(">= 50%")]
            LonHon50 = 1,
            [Description(">= 70%")]
            LonHon70 = 2
        }
        public enum DuongNuoiDuong
        {
            [Description("Đường miệng")]
            DuongMieng = 0,
            [Description("Ống thông")]
            OngThong = 1,
            [Description("Tĩnh mạch")]
            TinhMach = 2
        }
        public enum HoiChanDinhDuong
        {
            [Description("Không")]
            Khong = 0,
            [Description("Có")]
            Co = 1,
           
        }
        public enum TaiDanhGia
        {
            [Description("Sau 3 ngày")]
            SauBaNgay = 0,
            [Description("Sau 7 ngày")]
            SauBayNgay = 1,
        }
    }
}
