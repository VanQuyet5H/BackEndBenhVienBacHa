using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiTiepNhanNguoiBenh
        {
            [Description("NB có phiếu khám")]
            NguoiBenhCoPhieuKham = 1,
            [Description("NB chỉ làm DV")]
            NguoiBenhChiLamDichVu = 2
        }

        public enum BangKeChiPhiTheoNhomDichVu
        {
            [Description("Chi phí CLS ngoại trú")]
            CanLamSangNgoaiTru = 1,
            [Description("Chi phí CLS nội trú")]
            CanLamSangNoiTru = 2,
            [Description("Chi phí giường")]
            Giuong = 3,
            [Description("Chi phí thuốc")]
            Thuoc = 4,
            [Description("Chi phí VTYT")]
            VatTu = 5,
            [Description("Chi phí thuê phòng mổ")]
            ThuePhongMo = 6,
            [Description("Giảm đau")]
            GiamDau = 7,
            [Description("Test Covid")]
            TestCovid = 8,
            [Description("Dịch vụ khác")]
            SuatAn = 9,
        }

        public enum NoiDungBaoCaoHoatDongKhamBenh
        {
            [Description("1|Tổng số người khám")]
            TongSoNguoiKham = 1,
            [Description("a|Khám sức khỏe")]
            TongSoNguoiKhamSucKhoe = 2,
            [Description(" |T.đó: + Nội viện")]
            TongSoNguoiKhamSucKhoeNoiVien = 3,
            [Description("+ Ngoại viện")]
            TongSoNguoiKhamSucKhoeNgoaiVien = 4,
            [Description("b|Khám bệnh")]
            TongSoNguoiKhamBenh = 5,

            [Description("2|Tổng số lần")]
            TongSoLanKham = 6,
            [Description("a|Khám sức khỏe")]
            TongSoLanKhamSucKhoe = 7,
            [Description(" |T.đó: + Nội viện")]
            TongSoLanKhamSucKhoeNoiVien = 8,
            [Description("+ Ngoại viện")]
            TongSoLanKhamSucKhoeNgoaiVien = 9,
            [Description("b|Khám bệnh")]
            TongSoLanKhamBenh = 10,
            [Description(" |T.đó: + BHYT")]
            TongSoLanKhamBenhBHYT = 11,
            [Description("+ Viện phí")]
            TongSoLanKhamBenhVienPhi = 12,
            [Description("+ Không thu được")]
            TongSoLanKhamBenhKhongThuDuoc = 13,

            [Description("3|Trong TS số lần khám bệnh: ")]
            TrongTongSoLanKhamBenh = 14,
            [Description("a|Khám cấp cứu")]
            TrongTongSoLanKhamBenhCapCuu = 15,
            [Description("b|Trẻ em")]
            TrongTongSoLanKhamBenhTreEm = 16,
            [Description("T.đó <= 6 tuổi")]
            TrongTongSoLanKhamBenhTreEmDuoi6Tuoi= 17,
            [Description("c|Khám ngoại tỉnh")]
            TrongTongSoLanKhamBenhNgoaiTinh = 18,

            [Description("4|TS người bệnh chuyển viện")]
            TongNguoiBenhChuyenVien = 19,

            [Description("5|TS người bệnh tử vong")]
            TongNguoiBenhTuVong = 20,
            [Description("T.đó: + Trước 24 giờ")]
            TongNguoiBenhTuVongTruoc24H = 21,
            [Description("Tử vong ngoại viện")]
            TongNguoiBenhTuVongNgoaiVien = 22,


            [Description("6|TS người khám sàng lọc tiêm chủng")]
            TongNguoiBenhKhamSangLocTiemChung = 23,
            [Description("Tổng số người tiêm chủng")]
            TongNguoiBenhTiemChung = 24,
            [Description("Số mũi tiêm")]
            TongMuiTiem = 25,
        }

        public enum NhomDichVuKhamBenhBaoCaoHoatDongKhamBenh
        {
            [Description("CauHinhBaoCao.NhomKhamCapCuu")]
            KhamCapCuu = 1,
            [Description("CauHinhBaoCao.NhomKhamNoi")]
            KhamNoi = 2,
            [Description("CauHinhBaoCao.NhomKhamNhi")]
            KhamNhi = 3,
            [Description("CauHinhBaoCao.NhomKhamTMH")]
            KhamTMH = 4,
            [Description("CauHinhBaoCao.NhomKhamRHM")]
            KhamRHM = 5,
            [Description("CauHinhBaoCao.NhomKhamMat")]
            KhamMat = 6,
            [Description("CauHinhBaoCao.NhomKhamNgoai")]
            KhamNgoai = 7,
            [Description("CauHinhBaoCao.NhomKhamDaLieu")]
            KhamDaLieu = 8,
            [Description("CauHinhBaoCao.NhomKhamPhuSan")]
            KhamPhuSan = 9,
            [Description("CauHinhBaoCao.NhomKhamThamMy")]
            KhamThamMy = 10,
        }

        public enum NhomThuVienPhiChuaHoan
        {
            [Description("Ngoại Trú")]
            NgoaiTru = 1,
            [Description("Gói Dịch Vụ")]
            GoiDichVu = 2
        }
    }
}
