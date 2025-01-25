using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum HangBenhVien
        {
            [Description("Bệnh viện hạng 1")]
            BenhVienHang1 = 1,
            [Description("Bệnh viện hạng 2")]
            BenhVienHang2 = 2,
            [Description("Bệnh viện hạng 3")]
            BenhVienHang3 = 3,
            [Description("Bệnh viện hạng 4")]
            BenhVienHang4 = 4,
            [Description("Bệnh viện hạng đặc biệt")]
            BenhVienHangDacBiet = 5,
            [Description("Chưa xếp hạng")]
            BenhVienChuaXepHang = 6
        }

        public enum MucDoTuongTac
        {
            [Description("Cần theo dõi")]
            TheoDoi = 1,
            [Description("Cần thận trọng")]
            ThanTrong = 2,
            [Description("Cân nhắc nguy cơ/ lợi ích")]
            CanNhac = 3,
            [Description("Phối hợp nguy hiểm")]
            NguyHiem = 4
        }

        public enum LoaiDiUng
        {
            [Description("Thuốc")]
            Thuoc = 1,
            [Description("Thức ăn")]
            ThucAn = 2,
            [Description("Khác")]
            Khac = 3,
        }

        public enum MucDoChuYKhiChiDinh
        {
            [Description("Cần theo dõi")]
            TheoDoi = 1,
            [Description("Cần thận trọng")]
            ThanTrong = 2,
            [Description("Cân nhắc nguy cơ/ lợi ích")]
            CanNhac = 3,
            [Description("Chống chỉ định")]
            ChongChiDinh = 4
        }

        public enum LoaiThuocHoacHoatChat
        {
            [Description("Tân dược")]
            ThuocTanDuoc = 1,
            [Description("Chế phẩm YHCT")]
            ChePham = 2,
            [Description("Vị thuốc YHCT")]
            ViThuoc = 3,
            [Description("Phóng xạ")]
            PhongXa = 4,
            [Description("Tân dược tự bào chế")]
            TanDuocTuBaoChe = 5,
            [Description("Chế phẩm YHCT tự bào chế")]
            ChePhamTuBaoChe = 6


        }

        public enum LoaiThuocTheoQuanLy
        {
            [Description("Thuốc thường")]
            ThuocThuong = 1,
            [Description("Đông y")]
            DongY = 2,
            [Description("Gây nghiện")]
            GayNghien = 3,
            [Description("Hướng thần")]
            HuongThan = 4,
            [Description("Thuốc độc")]
            ThuocDoc = 5,
            [Description("Thuốc cấm sử dụng trong một số lĩnh vực")]
            ThuocCamTrongMotSoLinhVuc = 6,
            [Description("Thuốc điều trị lao")]
            ThuocDieuTriLao = 7,
            [Description("Thuốc phóng xạ")]
            ThuocPhongxa = 8,
            [Description("Thuốc corticoid")]
            ThuocCorticoid = 9,
            [Description("Thuốc điều trị ung thư dài ngày")]
            ThuocDieuTriUngThuDaiNgay = 10,
            [Description("Kháng sinh")]
            KhangSinh = 11,
        }

        public enum EnumDuocPhamBenhVienPhanNhom
        {
            [Description("Tân dược")]
            TanDuoc = 1,
            [Description("Thuốc từ dược liệu")]
            ThuocTuDuocLieu = 41,
            [Description("Hóa chất")]
            HoaChat = 43,
            [Description("Sinh phẩm")]
            SinhPham = 50,
            [Description("Mỹ phẩm")]
            MyPham = 56,
            [Description("Thực phẩm chức năng")]
            ThucPhamChucNang = 73,
            [Description("Vacxin")]
            Vacxin = 79,
            [Description("Thiết bị y tế")]
            ThietBiYTe = 85,
            [Description("Vật tư y tế")]
            VatTuYTe = 96,
            [Description("Chưa phân nhóm")]
            ChuaPhanNhom = 114,
            [Description("Sinh phẩm chẩn đoán")]
            SinhPhamChanDoan = 115,
        }

        public enum PhamViHuongBHYT
        {
            [Description("Thuốc trong phạm vi hưởng BHYT")]
            TrongPhamViHuong = 1,
            [Description("Thuốc ngoài phạm vi hưởng BHYT")]
            NgoaiPhamViHuong = 2
        }

        public enum PhanLoaiMau
        {
            [Description("Các chế phẩm có sử dụng dụng cụ, vật tư bổ sung")]
            ChePhamSuDungDungCuVatTuBoSung = 1,
            [Description("Các chế phẩm hồng cầu")]
            ChePhamHongCau = 2,
            [Description("Các chế phẩm huyết tương đông lạnh")]
            ChePhamHuyetTuongDongLanh = 3,
            [Description("Các chế phẩm huyết tương giàu tiểu cầu")]
            ChePhamHuyetTuongGiauTieuCau = 4,
            [Description("Các chế phẩm huyết tương tươi đông lạnh")]
            ChePhamHuyetTuongTuoiDongLanh = 5,
            [Description("Các chế phẩm khối tiểu cầu")]
            ChePhamKhoiTieuCau = 6,
            [Description("Các chế phẩm tủa lạnh")]
            ChePhamTuaLanh = 7,
            [Description("Các đơn vị máu toàn phần")]
            DonViMauToanPhan = 8,
            [Description("Các khối bạch cầu")]
            KhoiBachCau = 9
        }

        public enum TuyenChuyenMonKyThuat
        {
            [Description("Tuyến 1")]
            Tuyen1 = 1,
            [Description("Tuyến 2")]
            Tuyen2 = 2,
            [Description("Tuyến 3")]
            Tuyen3 = 3,
            [Description("Tuyến 4")]
            Tuyen4 = 4,
            [Description("Chưa Phân Tuyến")]
            ChuaPhanTuyen = 5,
        }

        public enum InputStringStoredKey
        {
            [Description("Thuốc")]
            Thuoc = 1,
            [Description("Vật Tư")]
            VatTu = 2,
            [Description("Kỹ Thuật")]
            KyThuat = 3,
            [Description("Nhóm Đối Tượng Khám Sức Khỏe")]
            NhomDoiTuongKhamSucKhoe = 4,
            [Description("Ghi Chú CLS")]
            GhiChuCanLamSang = 5,
            [Description("Lý do nhập viện")]
            LyDoNhapVien = 6,
            [Description("Ghi chú giấy ra viện")]
            GhiChuGiayRaVien = 7
           
        }
        public enum LoaiNoiChiDinh
        {
            [Description("Nội trú phiếu điều trị")]
            NoiTruPhieuDieuTri = 1,
            [Description("Phẫu thuật thủ thuật")]
            PhauThuatThuThuat = 2,
            [Description("Chẩn đoán hình ảnh")]
            ChanDoanHinhAnh = 3,
            [Description("Yêu cầu khám bệnh")]
            YeuCauKhamBenh = 4,
            [Description("Tiêm chủng")]
            TiemChung = 5
        }

        public enum KieuHienThiICD
        {
            [Description("Mã - ghi chú")]
            MaGachNgangGhiChu = 1,
            [Description("Tên - ghi chú")]
            TenGachNgangGhiChu = 2,
            [Description("Mã (ghi chú)")]
            MaNgoacTronGhiChu = 3,
            [Description("Tên (ghi chú)")]
            TenNgoacTronGhiChu = 4,
            [Description("Mã - tên")]
            MaGachNgangTen = 5,
            [Description("Mã - tên (ghi chú)")]
            MaGachNgangTenNgoacTronGhiChu = 6,
            [Description("Mã - tên: ghi chú")]
            MaGachNgangTenHaiChamGhiChu = 7
        }

        public enum LoaiDieuKienBaoQuanDuocPham
        {
            [Description("< 0°C")]
            Duoi0Do = 1,
            [Description("2°C - 8°C")]
            Tu2Den8Do = 2,
            [Description("15°C - 25°C")]
            Tu15Den25Do = 3,
            [Description("Khác")]
            Khac = 4
        }
    }
}
