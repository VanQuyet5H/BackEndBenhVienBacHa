using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiLichSuKhamChuaBenh
        {
            [Description("Khám bệnh")]
            KhamBenh = 1,
            [Description("Cận lâm sàng")]
            CanLamSang = 2,
            [Description("Y lệnh")]
            YLenh = 3,
        }

        public enum LoaiLichSuKhamChuaBenhChiTiet
        {
            [Description("Dịch vụ khám bệnh")]
            DichVuKhamBenh = 1,
            [Description("Chẩn đoán hình ảnh - thăm dò chức năng")]
            DichVuCDHATDCN = 2,
            [Description("Dịch vụ xét nghiệm")]
            DichVuXetNghiem = 3,
            [Description("Phiếu điều trị")]
            PhieuDieuTri = 4,
            [Description("Phiếu chăm sóc")]
            PhieuChamSoc = 5,
            [Description("Hồ sơ khác")]
            HoSoKhac = 6,
            [Description("Đơn thuốc")]
            DonThuoc = 7,
            [Description("Đơn vật tư")]
            DonVatTu = 8,
        }
    }
}
