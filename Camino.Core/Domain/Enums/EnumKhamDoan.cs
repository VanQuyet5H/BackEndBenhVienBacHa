using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum HinhThucKhamBenh
        {
            [Description("Nội viện")]
            NoiVien = 1,
            [Description("Ngoại viện")]
            KhamDoanNgoaiVien = 2
        }

        public enum LoaiHopDong
        {
            [Description("Khám sức khỏe công ty")]
            KhamSucKhoeCongTy = 1
        }

        public enum TrangThaiHopDongKham
        {
            [Description("Đang thực hiện")]
            DangThucHienHD = 1,
            [Description("Đã kết thúc")]
            DaKetThucHD = 2
        }

        public enum ChuyenKhoaKhamSucKhoe
        {
            [Description("Nội khoa")]
            NoiKhoa = 1,
            [Description("Ngoại khoa")]
            NgoaiKhoa = 2,
            [Description("Sản phụ khoa")]
            SanPhuKhoa = 3,
            [Description("Mắt")]
            Mat = 4,
            [Description("Tai mũi họng")]
            TaiMuiHong = 5,
            [Description("Răng hàm mặt")]
            RangHamMat = 6,
            [Description("Da liễu")]
            DaLieu = 7
        }

        public enum TinhTrangHonNhan
        {
            [Description("Có gia đình")]
            CoGiaDinh = 1,
            [Description("Chưa có gia đình")]
            ChuaCoGiaDinh = 2
        }

        public enum CongViecKhamSucKhoe
        {
            [Description("Lấy Mẫu")]
            LayMau = 1,
            [Description("Khám")]
            Kham = 2,
            [Description("Khám + Lấy Mẫu")]
            KhamVaLayMau = 3
        }

        public enum DoiTuongNhanSu
        {
            [Description("Fulltime")]
            Fulltime = 1,
            [Description("Parttime")]
            Parttime = 2
        }

        public enum NhomDichVuChiDinhKhamSucKhoe
        {
            [Description("Khám bệnh")]
            KhamBenh = 1,
            [Description("Xét nghiệm")]
            XetNghiem = 2,
            [Description("Chẩn đoán hình ảnh")]
            ChuanDoanHinhAnh = 3,
            [Description("Thăm dò chức năng")]
            ThamDoChucNang = 4,
            [Description("Khác")]
            KH = 5,
        }

        public enum NguonNhanSu
        {
            [Description("Nội viện")]
            NoiVien = 1,
            [Description("Ngoại viện")]
            NgoaiVien = 2
        }

        public enum LoaiHoSoKhamSucKhoe
        {
            [Description("Sổ KSK định kỳ")]
            SoKhamSucKhoeDinhKy = 1,
            [Description("Phiếu đăng ký KSK")]
            PhieuDangKyKhamSucKhoe = 2,
            [Description("Bảng hướng dẫn KSK")]
            BangHuongDanKhamSucKhoe = 3
        }

        public enum PhanLoaiSucKhoe
        {
            [Description("Loại I")]
            Loai1 = 1,
            [Description("Loại II")]
            Loai2 = 2,
            [Description("Loại III")]
            Loai3 = 3,
            [Description("Loại IV")]
            Loai4 = 4,
            [Description("Loại V")]
            Loai5 = 5
        }

        public enum TinhTrangDoChiSoSinhTon
        {
            [Description("Chưa đo")]
            ChuaDo = 1,
            [Description("Đã đo")]
            DaDo = 2
        }

        public enum ChuyenKhoaKhamSucKhoeChinh
        {
            [Description("Nội khoa")]
            NoiKhoa = 1,
            [Description("Ngoại khoa")]
            NgoaiKhoa = 2,
            //[Description("Sản phụ khoa")]
            //SanPhuKhoa = 3,
            [Description("Mắt")]
            Mat = 4,
            [Description("Tai mũi họng")]
            TaiMuiHong = 5,
            [Description("Răng hàm mặt")]
            RangHamMat = 6,
            [Description("Da liễu")]
            DaLieu = 7
        }
    }
}
