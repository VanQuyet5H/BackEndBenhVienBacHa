using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumCauHinh
        {
            [Description("bool")]
            Boolean = 1,
            [Description("int")]
            Integer = 2,
            [Description("string")]
            String = 3,
            [Description("double")]
            Double = 4,
            [Description("datetime")]
            Datetime = 5,
            [Description("phone")]
            Phone = 6,
            [Description("email")]
            Email = 7
        }

        public enum LoaiCauHinh
        {
            [Description("Tất cả")]
            TatCa = 0,
            [Description("Cấu hình bảo hiểm y tế")]
            BaoHiemYTe = 1,
            [Description("Cấu hình hệ thống")]
            CauHinhHeThong = 2,
            [Description("Cấu hình tiếp nhận")]
            CauHinhTiepNhan = 3,
            [Description("Cấu hình báo cáo")]
            CauHinhBaoCao = 4,
            [Description("Cấu hình gạch nợ")]
            CauHinhGachNo = 5,
            [Description("Cấu hình xét nghiệm")]
            CauHinhXetNghiem = 6,
            [Description("Cấu hình nội trú")]
            CauHinhNoiTru = 7,
            [Description("Cấu hình phiếu thu")]
            CauHinhPhieuThu = 8,
            [Description("Cấu hình dịch vụ kỹ thuật")]
            CauHinhDichVuKyThuat = 9,
            [Description("Cấu hình dịch vụ khám bệnh")]
            CauHinhDichVuKhamBenh = 10,
            [Description("Cấu hình CĐHA-TDCN")]
            CauHinhCDHA = 11,
            [Description("Cấu hình tiêm chủng")]
            CauHinhTiemChung = 12,
            [Description("Cấu hình khám sức khỏe")]
            CauHinhKhamSucKhoe = 13,
            [Description("Cấu hình khám bệnh")]
            CauHinhKhamBenh = 14,
        }
        public enum LoaiThapGia
        {
            [Description("Tháp giá thuốc không bảo hiểm")]
            ThuocKhongBaoHiem = 1,
            [Description("Tháp giá vật tư tiêu hao")]
            VatTuTieuHao = 2,
            [Description("Tháp giá vật tư thay thế")]
            VatTuThayThe = 3
        }
    }
}
