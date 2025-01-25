using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiKetLuanKhamSangLocTiemChung
        {
            [Description("Đủ điều kiện tiêm")]
            DuDieuKienTiem = 1,
            [Description("Tạm hoãn tiêm chủng")]
            TamHoanTiemChung = 2,
            [Description("Không đồng ý tiêm")]
            KhongDongYTiem = 3,
            [Description("Chống chỉ định")]
            ChongChiDinh = 4
        }
        public enum LoaiPhanUngSauTiem
        {
            [Description("Không có phản ứng")]
            KhongCoPhanUng = 1,
            [Description("Phản ứng thông thường")]
            PhanUngThongThuong = 2,
            [Description("Tai biến nặng")]
            TaiBienNang = 3,
            [Description("Phản ứng khác")]
            PhanUngKhac = 4
        }
        public enum ViTriTiem
        {
            [Description("Tiêm bắp")]
            TiemBap = 1,
            [Description("Tiêm tĩnh mạch")]
            TiemTinhMach = 2,
            [Description("Tiêm dưới da")]
            TiemDuoiDa = 3,
            [Description("Tiêm trong da")]
            TiemTrongDa = 4,
            [Description("Uống")]
            Uong = 5
        }
        public enum TrangThaiTiemChung
        {
            [Description("Chưa tiêm chủng")]
            ChuaTiemChung = 1,
            [Description("Đã tiêm chủng")]
            DaTiemChung = 2,
            //[Description("Chống chỉ định")]
            //ChongChiDinh = 3,
            //[Description("Không đồng ý tiêm")]
            //KhongDongYTiem = 4
        }

        public enum NhomKhamSangLoc
        {
            [Description("Ngoài bệnh viện")]
            NgoaiBenhVien = 1,
            [Description("Trong bệnh viện")]
            TrongBenhVien = 2,
            [Description("Covid-19")]
            Covid19 = 3
        }
    }
}
