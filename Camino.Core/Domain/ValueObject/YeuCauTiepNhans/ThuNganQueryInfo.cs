using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class PhieuThuNgan
    {
        public long Id { get; set; }
        public string Html { get; set; }
        public string TenFile { get; set; }

        public string PageSize { get; set; } //A0,A1,A2,A3,A4,A5
        public string PageOrientation { get; set; } //Landscape,Portrait
    }

    public class ThuNganQueryInfo : QueryInfo
    {
        public bool ChuaThu { get; set; }
        public bool CongNo { get; set; }
        public bool HoanUng { get; set; }
        public bool DaThu { get; set; }
    }

    public class ThuNganNoiTruQueryInfo : QueryInfo
    {
        public bool ChoQuyetToan { get; set; }
        public bool ChoTamUngThem { get; set; }
        public bool DaTamUng { get; set; }
        public bool ChuaTaoBenhAn { get; set; }
    }

    public class ThuNganNoiTruDaQuyetToanQueryInfo : QueryInfo
    {
        public bool CongNo { get; set; }
        public bool HoanTien { get; set; }
        public bool DaQuyetToan { get; set; }
    }

    public class ThuNganMarketingQueryInfo : QueryInfo
    {
        public bool ChuaThanhToan { get; set; }
        public bool DangThanhToan { get; set; }
    }

    public class ThongTinPhieuMarketingQueryInfo : QueryInfo
    {
        public long BenhNhanId { get; set; }
        public long? GoiDichDichVuId { get; set; }

        public DateTime? ThoiDiemDenFormat { get; set; }
        public DateTime? ThoiDiemTuFormat { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
