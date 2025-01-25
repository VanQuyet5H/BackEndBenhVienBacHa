using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumLoaiKhoDuocPham
        {
            //[Description("Kho tổng")]
            //KhoTong = 1,
            //[Description("Kho ngoại")]
            //KhoNgoai = 2,
            //[Description("Kho nội")]
            //KhoNoi = 3,
            //[Description("Kho chờ xử lý")]
            //KhoChoXuLy = 4,
            [Description("Kho tổng dược phẩm cấp I")]
            KhoTongDuocPhamCap1 = 1,
            [Description("Kho tổng dược phẩm cấp II")]
            KhoTongDuocPhamCap2 = 2,
            [Description("Kho tổng VTYT cấp I")]
            KhoTongVTYTCap1 = 3,
            [Description("Kho tổng VTYT cấp II")]
            KhoTongVTYTCap2 = 4,
            [Description("Kho lẻ")]
            KhoLe = 5,
            [Description("Nhà thuốc")]
            NhaThuoc = 6,
            [Description("Kho dược phẩm chờ xử lý")]
            KhoDuocPhamChoXuLy = 7,
            [Description("Kho VTYT chờ xử lý")]
            KhoVTYTChoXuLy = 8,
            [Description("Kho Vắcxin")]
            KhoVacXin = 9,
            [Description("Kho hành chính")]
            KhoHanhChinh = 10,
            [Description("Kho KSNK")]
            KhoKSNK = 11
        }
        public enum EnumKhoDuocPham
        {
            //[Description("Kho thuốc BHYT")]
            //KhoThuocBHYT = 20,
            //[Description("Kho thuốc viện phí")]
            //KhoThuocVienPhi = 21,
            [Description("Kho nhà thuốc")]
            KhoNhaThuoc = 6,
            [Description("Kho Thuốc BHYT")]
            KhoThuocBHYT = 4,
            [Description("Kho Thuốc Bệnh Viện")]
            KhoThuocBenhVien = 3,
            [Description("Kho Hóa chất")]
            KhoHoaChat = 2,
            [Description("Kho tổng Dược phẩm")]
            KhoTongDuocPham = 1,
            [Description("Kho Vật Tư y tế")]
            KhoVatTuYTe = 7,
        }

        public enum PhuongPhapTinhGiaTriTonKho
        {
            ApVAT = 1,
            KhongApVAT = 2,
        }
    }
}
