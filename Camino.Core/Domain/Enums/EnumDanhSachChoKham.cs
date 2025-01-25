using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumTrangThaiYeuCauKhamBenh
        {
            [Description("Chưa khám")]
            ChuaKham = 1,
            [Description("Đang khám")]
            DangKham = 2,
            [Description("Đang làm chỉ định")]
            DangLamChiDinh = 3,
            [Description("Đang đợi kết luận")]
            DangDoiKetLuan = 4,
            [Description("Đã khám")]
            DaKham = 5,
            [Description("Hủy khám")]
            HuyKham = 6
        }

        public enum EnumTinhTrangThe
        {
            [Description("Còn hiệu lực")]
            ConHieuLuc = 1,
            [Description("Hết hiệu lực")]
            HetHieuLuc = 2,
            [Description("Không xác định")]
            KhongXacDinh = 3,
        }

        public enum EnumTrangThaiHangDoi
        {
            [Description("Chờ khám")]
            ChoKham = 1,
            [Description("Đang khám")]
            DangKham = 2
        }

        public enum EnumLoaiHangDoi
        {
            [Description("Chuẩn bị khám")]
            ChuanBiKham = 1,
            [Description("Làm chỉ định")]
            LamChiDinh = 2,
            [Description("Đợi kết luận")]
            DoiKetLuan = 3
        }

        public enum EnumLoaiDonThuoc
        {
            [Description("Thuốc BHYT")]
            ThuocBHYT = 1,
            [Description("Thuốc Không BHYT")]
            ThuocKhongBHYT = 2,
        }
    }
}
