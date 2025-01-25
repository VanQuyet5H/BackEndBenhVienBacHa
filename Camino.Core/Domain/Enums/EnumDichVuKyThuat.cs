using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiPhauThuatThuThuat
        {
            [Description("Thủ thuật loại 1")]
            ThuThuatLoai1 = 1,
            [Description("Thủ thuật loại 2")]
            ThuThuatLoai2 = 2,
            [Description("Thủ thuật loại 3")]
            ThuThuatLoai3 = 3,
            [Description("Thủ thuật loại đặc biệt")]
            ThuThuatLoaiDacBiet = 4,
            [Description("Phẫu thuật loại 1")]
            PhauThuatLoai1 = 5,
            [Description("Phẫu thuật loại 2")]
            PhauThuatLoai2 = 6,
            [Description("Phẫu thuật loại 3")]
            PhauThuatLoai3 = 7,
            [Description("Phẫu thuật loại đặc biệt")]
            PhauThuatLoaiDacBiet = 8
        }

        //Update Phân loại PTTT (BVHD-3146)
        public enum LoaiPTTT
        {
            [Description("Loại I")]
            Loai1 = 1,
            [Description("Loại II")]
            Loai2 = 2,
            [Description("Loại III")]
            Loai3 = 3,
            [Description("Đặc biệt")]
            DacBiet = 4,
            [Description("Chưa phân loại")]
            ChuaPhanLoai = 5
        }

        public enum LoaiDichVuKyThuat
        {
            [Description("Phẫu thuật thủ thuật")]
            ThuThuatPhauThuat = 1,
            [Description("Xét nghiệm")]
            XetNghiem = 2,
            [Description("Chẩn đoán hình ảnh")]
            ChuanDoanHinhAnh = 3,
            [Description("Thăm dò chức năng")]
            ThamDoChucNang = 4,
            [Description("DỊCH VỤ THEO YÊU CẦU")]
            TheoYeuCau = 5,
            [Description("Khác")]
            Khac = 6,
            [Description("Suất ăn")]
            SuatAn = 200,
            [Description("Sàng lọc tiêm chủng")]
            SangLocTiemChung = 300
        }

        public enum LoaiNoiThucHienUuTien
        {
            [Description("Hệ thống")]
            HeThong = 1,
            [Description("Người dùng")]
            NguoiDung = 2
        }

        public enum BuaAn
        {
            [Description("Sáng")]
            Sang = 1,
            [Description("Trưa")]
            Trua = 2,
            [Description("Chiều")]
            Chieu = 3,
            [Description("Tối")]
            Toi = 4,
            [Description("Khuya")]
            Khuya = 5
        }
    }
}
