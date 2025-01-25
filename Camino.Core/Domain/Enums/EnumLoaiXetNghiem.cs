using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiXetNghiem
        {
            [Description("Hóa Sinh Nước Tiểu Phân Dịch Chọc Dò")]
            HoaSinhNuocTieuPhanDichChocDo = 1,
            [Description("Huyết Học")]
            HuyetHoc = 2,
            [Description("Tủy Đồ")]
            TuyDo = 3,
            [Description("Định Nhóm Và Truyền Máu")]
            DinhNhomVaTruyenMau = 4,
            [Description("Đông Máu")]
            DongMau = 5,
            [Description("Hóa Sinh Máu")]
            HoaSinhMau = 6, 
            [Description("Huyết Đồ")]
            HuyetDo = 7,
            [Description("Hóa Sinh")]
            HoaSinh = 8,
            [Description("HIV")]
            HIV = 9,
            [Description("Ký Sinh Trùng Đường Ruột")]
            KySinhTrungDuongRuot = 10,
            [Description("Nội Tiết")]
            NoiTiet = 11,
            [Description("Nước Tiểu")]
            NuocTieu = 12,
            [Description("Sinh Hóa Tủy Xương")]
            SinhHoaTuyXuong = 13,
            [Description("Tinh Dịch Đồ")]
            TinhDichDo = 14,
            [Description("Tế Bào Máu Ngoại Vi")]
            TeBaoMauNgoaiVi = 15,
            [Description("Vi Sinh")]
            ViSinh = 16,
            [Description("Xét Nghiệm Giải Phẫu Bệnh")]
            XetNghiemGiaiPhau = 17,
            [Description("Xét Nghiệm Test Phản Ứng Lao")]
            XetNghiemTestPhanUngLao = 18,
            [Description("Xét Nghiệm Sốt Rét")]
            XetNghiemSotRet = 19,
            [Description("Khác")]
            Khac = 20,
        }
    }
}
