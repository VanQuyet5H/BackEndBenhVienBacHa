using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum TrangThaiBenhAn
        {
            [Description("Chờ q.toán v.phí ngoại trú")]
            ChoQuyetToan = 1,
            [Description("Chưa tạo BA")]
            ChuaTaoBenhAn = 2,
            [Description("Đã tạo BA")]
            DaTaoBenhAn = 3
        }

        public enum LoaiBenhAn
        {
            [Description("BA Nội Khoa")]
            NoiKhoa = 1,
            [Description("BA Nhi Khoa")]
            NhiKhoa = 2,
            [Description("BA Phụ Khoa")]
            PhuKhoa = 3,
            [Description("BA Sản Khoa (Mổ)")]
            SanKhoaMo = 4,
            [Description("BA Sản Khoa (Thường)")]
            SanKhoaThuong = 5,
            [Description("BA Ngoại Khoa")]
            NgoaiKhoa = 6,
            [Description("BA Thẩm Mỹ")]
            ThamMy = 7,
            [Description("Trẻ Sơ Sinh")]
            TreSoSinh = 8
        }

        public enum TrangThaiDieuTri
        {
            [Description("Chờ nhập viện")]
            ChoNhapVien = 1,
            [Description("Đang điều trị")]
            DangDieuTri = 2,
            [Description("Đã ra viện")]
            DaRaVien = 3,
            [Description("Chuyển viện")]
            ChuyenVien = 4
        }
    }
}
