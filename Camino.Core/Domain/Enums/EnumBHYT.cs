using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {

        public enum EnumLyDoVaoVien
        {
            [Description("Đúng tuyến")]
            DungTuyen = 1,
            [Description("Cấp cứu")]
            CapCuu = 2,
            [Description("Trái tuyến")]
            TraiTuyen = 3,
            [Description("Thông tuyến")]
            ThongTuyen = 4,
        }
        public enum EnumLoaiGiuong
        {
            [Description("Nội")]
            Noi = 1,
            [Description("Ngoại")]
            Ngoai = 2,
            [Description("Cả 2")]
            CaHai = 3

        }
        public enum EnumMaHoaHinhThucKCB
        {
            [Description("Khám bệnh")]
            KhamBenh = 1,
            [Description("Điều trị ngoại trú")]
            DieuTriNgoaiTru = 2,
            [Description("Điều trị nội trú")]
            DieuTriNoiTru = 3
        }
        public enum EnumMaTaiNan
        {
            [Description("Không")]
            Khong = 0,
            [Description("Tai nạn giao thông")]
            TaiNanGiaoThong = 1,
            [Description("Tai nạn lao động")]
            TaiNanLaoDong = 2,
            [Description("Tai nạn dưới nước")]
            TaiNanDuoiNuoc = 3,
            [Description("Bỏng")]
            Bong = 4,
            [Description("Bạo lực, xung đột")]
            BaoLucXungDot = 5,
            [Description("Tự tử")]
            TuTu = 6,
            [Description("Ngộ độc các loại")]
            NgoDocCacLoai = 7,
            [Description("Khác")]
            Khac = 8,
        }
        public enum EnumKetQuaDieuTri
        {
            [Description("Khỏi")]
            Khoi = 1,
            [Description("Đỡ")]
            Do = 2,
            [Description("Không thay đổi")]
            KhongThayDoi = 3,
            [Description("Nặng hơn")]
            NangHon = 4,
            [Description("Tử vong")]
            TuVong = 5
        }
        public enum EnumTinhTrangRaVien
        {
            [Description("Ra viện")]
            RaVien = 1,
            [Description("Chuyển viện")]
            ChuyenVien = 2,
            [Description("Trốn viện")]
            TronVien = 3,
            [Description("Xin ra viện")]
            XinRaVien = 4
        }
        public enum EnumHinhThucRaVien
        {
            [Description("Ra viện")]
            RaVien = 1,
            [Description("Xin về")]
            XinVe = 2,
            [Description("Bỏ về")]
            BoVe = 3,
            [Description("Chuyển viện")]
            ChuyenVien = 4,
            [Description("Tử vong")]
            TuVong = 5,
            [Description("Không đồng ý nhập viện")]
            KhongDongYNhapVien = 6,
            [Description("Nặng xin về")]
            NangXinVe = 7,
            [Description("Tử vong trước 24h")]
            TuVongTruoc24H = 8,
            [Description("Cấp cứu ra viện trong ngày")]
            CapCuuRaVienTrongNgay = 9
        }
        public enum EnumMaPhuongThucThanhToan
        {
            [Description("Phí dịch vụ")]
            PhiDichVu = 0,
            [Description("Định suất")]
            DinhSuat = 1,
            [Description("Ngoài định suất")]
            NgoaiDinhSuat = 2,
            [Description("DRG")]
            DRG = 3
        }
        public enum EnumTrangThaiDonThuoc
        {
            [Description("Chưa cấp thuốc")]
            ChuaCapThuoc = 1,
            [Description("Đã cấp thuốc")]
            DaCapThuoc = 2,
        }
        public enum EnumTrangThaiDonVTYT
        {
            [Description("Chưa cấp VTYT")]
            ChuaCapVTYT = 1,
            [Description("Đã cấp VTYT")]
            DaCapVTYT = 2,
        }
        public enum EnumThoiGianDonThuoc
        {
            [Description("1")]
            Mot = 1,
            [Description("2")]
            Hai = 2,
            [Description("3")]
            Ba = 3,
            [Description("4")]
            Bon = 4,
            [Description("5")]
            Nam = 5,
            [Description("6")]
            Sau = 6,
            [Description("7")]
            Bay = 7,
            [Description("8")]
            Tam = 8,
            [Description("9")]
            Chin = 9,
            [Description("10")]
            Muoi = 10,
            [Description("1/2")]
            MotPhanHai = 11,
            [Description("1/3")]
            MotPhanBa = 12,
            [Description("1/4")]
            MotPhanTu = 13,
        }
        public enum EnumDanhMucNhomTheoChiPhi
        {
            [Description("Xét nghiệm")]
            XetNghiem = 1,
            [Description("Chẩn đoán hình ảnh")]
            ChuanDoanHinhAnh = 2,
            [Description("Thăm dò chức năng")]
            ThamDoChucNang = 3,
            [Description("Thuốc trong danh mục BHYT")]
            ThuocTrongDanhMucBHYT = 4,
            [Description("Thuốc điều trị ung thư, chống thải ghép ngoài danh mục")]
            ThuocDieuTriUngThuVaChongThaiGhepNgoaiDanhMuc = 5,
            [Description("Thuốc thanh toán theo tỷ lệ")]
            ThuocThanhToanTheoTyLe = 6,
            [Description("Máu và chế phẩm máu")]
            MauVaChePhamMau = 7,
            //update 14/022022: QĐ 5937/QĐ-BYT ThuThuatPhauThuat -> PhauThuat
            [Description("Phẫu thuật")]
            ThuThuatPhauThuat = 8,
            [Description("DVKT thanh toán theo tỷ lệ")]
            DVKTThanhToanTheoTyLe = 9,
            [Description("Vật tư y tế trong danh mục BHYT")]
            VatTuYTeTrongDanhMucBHYT = 10,
            [Description("VTYT thanh toán theo tỷ lệ")]
            VTYTThanhToanTheoTyLe = 11,
            [Description("Vận chuyển")]
            VanChuyen = 12,
            [Description("Khám bệnh")]
            KhamBenh = 13,
            [Description("Giường điều trị ngoại trú")]
            GiuongDieuTriNgoaiTru = 14,
            [Description("Giường điều trị nội trú")]
            GiuongDieuTriNoiTru = 15,
            //update 14/022022: QĐ 5937/QĐ-BYT
            [Description("Thủ thuật")]
            ThuThuat = 18,
        }

        public enum HinhThucThanhToanPhi
        {
            [Description("Tiền mặt")]
            TienMat = 1,
            [Description("Chuyển khoản")]
            ChuyenKhoan = 2,
            [Description("Pos")]
            Pos = 3,
            [Description("Công nợ")]
            CongNo = 4
        }

        public enum LoaiChuyenTuyen
        {
            [Description("Cùng tuyến hạng 1")]
            CungTuyenHang1 = 1,
            [Description("Cùng tuyến hạng 2")]
            CungTuyenHang2 = 2,
            [Description("Từ tuyến trên về")]
            TuTuyenTrenVe = 3,
            [Description("Tuyến dưới liền kề")]
            TuyenDuoiLienKe = 4,
            [Description("Tuyến dưới không liền kề")]
            TuyenDuoiKhongLienKe = 5,
            [Description("Nơi khác chuyển đến")]
            NoiKhacChuyenDen = 6,
            [Description("Tuyến dưới chuyển lên")]
            TuyenDuoiChuyenLen = 7,
            [Description("Chuyển người bệnh từ tuyến dưới lên tuyến trên liền kề")]
            TuTuyenDuoiLenTrenLienKe = 8,
            [Description("Chuyển người bệnh từ tuyến dưới lên tuyến trên không qua tuyến liền kề")]
            TuTuyenDuoiLenTrenKhongLienKe = 9,
            [Description("Chuyển người bệnh từ tuyến trên về tuyến dưới")]
            TuTuyenTrenVeTuyenDuoi = 10,
            [Description("Chuyển người bệnh giữa các cơ sở khám bệnh, chữa bệnh trong cùng tuyến")]
            ChuyenGiuaCacCoSoCungTuyen = 11,
            [Description("Tuyến trên chuyển về cơ sơ kkcb nơi gửi người bệnh để tiếp tục điều trị")]
            TuyenTrenChuyenVeCoSoDaGoi = 12
        }

        public enum LyDoTuVong
        {
            [Description("Do bệnh")]
            DoBenh = 1,
            [Description("Do tai biến điều trị")]
            DoTaiBienDieuTri = 2,
            [Description("Do ung thư")]
            DoUngThu = 3,
            [Description("Do nguyên nhân khác")]
            DoNguyenNhanKhac = 4,
            [Description("Không rõ")]
            KhongRo = 5,
        }

        public enum DieuKienDuChuyenTuyen
        {
            [Description("Theo yêu cầu của người bệnh")]
            TheoYeuCauCuaNguoiBenh = 1,
            [Description("Người đại diện hợp pháp")]
            NguoiDaiDienHopPhap = 2,
        }

        public enum LoaiGiaPhauThuat
        {
            //[Description("Trống")]
            //Trong = 1,
            [Description("Lành tính")]
            LanhTinh = 1,
            [Description("Nghi ngờ")]
            NghiNgo = 2,
            [Description("Ác tính")]
            AcTinh = 3
        }

        public enum TienThaiPara
        {
            [Description("Sinh(đủ tháng)")]
            SinhDuThang = 1,
            [Description("Sớm(đẻ non)")]
            Som = 2,
            [Description("Sẩy(nạo,hút)")]
            Say = 3,
            [Description("Sống")]
            Song = 4,
        }

        public enum DoLot
        {
            [Description("Cao")]
            Cao = 1,
            [Description("Chúc")]
            Chuc = 2,
            [Description("Chặt")]
            Chặt = 3,
            [Description("Lọt")]
            Lọt = 4
        }


        public enum TinhTrangVoOi
        {
            [Description("Ối phồng")]
            OiPhong = 1,
            [Description("Ối dẹt")]
            OiDep = 2,
            [Description("Ối quả lê")]
            OiQuaLe = 3
        }

        public enum VoOi
        {
            [Description("Tự nhiên")]
            TuNhien = 1,
            [Description("Bấm ói")]
            BamOi = 2
        }

        public enum TangSinhMon
        {
            [Description("Không rách")]
            KhongRach = 1,
            [Description("Rách")]
            Rach = 2,
            [Description("Cắt")]
            Cat = 3
        }

        public enum CoTuCung
        {
            [Description("Không rách")]
            KhongRach = 1,
            [Description("Rách")]
            Rach = 2
        }

        public enum PhuongPhapDe
        {
            [Description("Đẻ thường")]
            DeThuong = 1,
            [Description("forceps")]
            Forceps = 2,
            [Description("Giác hút")]
            GiacHut = 3,
            [Description("PT")]
            PT = 4,
            [Description("Đẻ chỉ huy")]
            DeChiHuy = 5,
            [Description("Khác")]
            Khac = 6
        }

        public enum TinhTrangSoSinh
        {
            [Description("Khóc ngay")]
            KhocNgay = 1,
            [Description("Ngạt")]
            Ngat = 2,
            [Description("Khác")]
            Khac = 3
        }

        public enum MauSacDa
        {
            [Description("Hồng hào")]
            HongHao = 1,
            [Description("Xanh tái")]
            XanhTai = 2,
            [Description("Vàng")]
            Vang = 3,
            [Description("Tím")]
            Tim = 4,
            [Description("Khác")]
            Khac = 5
        }

        public enum TinhTrangKhiSinh
        {
            [Description("Đẻ thường")]
            DeThuong = 1,
            [Description("Forces")]
            Forces = 2,
            [Description("Giác hút")]
            GiacHut = 3,
            [Description("Đẻ phẩu thuật")]
            DePhauThuat = 4,
            [Description("Đẻ chỉ huy")]
            DeChiHuy = 5,
            [Description("Khác")]
            Khac = 6
        }
        public enum DeThuong
        {
            [Description("Sữa mẹ")]
            SuaMe = 1,
            [Description("Nuôi nhân tạo")]
            NuoiNhanTao = 2,
            [Description("Hổn hợp")]
            HonHop = 3
        }

        public enum ChamSoc
        {
            [Description("Tại vườn trẻ")]
            TaiVuonTre = 1,
            [Description("Tại nhà")]
            TaiNha = 2
        }

        public enum EnumTrangThaiDieuTriNoiTru
        {
            [Description("Đang Điều Trị")]
            DangDieuTri = 1,
            [Description("Đã Ra Viện")]
            DaRaVien = 2,
            [Description("Chuyễn Viện")]
            ChuyenVien = 3

        }

        public enum EnumGioiTinh
        {
            [Description("Trai")]
            Trai = 1,
            [Description("Gái")]
            Gai = 2
        }

        public enum EnumTrangThaiSong
        {
            [Description("Sống")]
            Song  = 1,
            [Description("Chết")]
            Chet = 2
        }

        public enum EnumLyDoChuyenTuyen
        {
            [Description("Theo yêu cầu của người bệnh")]
            TheoYeuCauNguoiBenh = 1,
            [Description("Người đại diện hợp pháp")]
            NguoiDaiDienHopPhap = 2
        }
    }
}

