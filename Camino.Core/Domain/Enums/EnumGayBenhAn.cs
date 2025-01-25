using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiPhieuHoSoBenhAnDienTu
        {
            [Description("Bìa bệnh án")]
            BiaBenhAn = 1,
            [Description("Hành chính - Bệnh án")]
            HanhChinhBenhAn = 2,
            [Description("Tài liệu đợt điều trị trước/ tuyến trước")]
            TaiLieuDieuTriLanTruoc = 3,
            [Description("Phiếu khám bệnh vào viện")]
            PhieuKhamBenhVaoVien = 4,
            [Description("Phiếu chỉ định")]
            PhieuChiDinh = 5,
            [Description("Nhóm dịch vụ CLS")]
            NhomDichVuCLS = 6,
            [Description("Phiếu khám chuyên khoa")]
            PhieuKhamTheoChuyenKhoa = 7,
            [Description("Hồ sơ khác")]
            HoSoKhac = 8,
            [Description("Tờ điều trị")]
            ToDieuTri = 9,
        }
    }
}
