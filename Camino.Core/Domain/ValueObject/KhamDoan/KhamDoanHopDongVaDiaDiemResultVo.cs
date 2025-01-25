using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KhamDoanHopDongVaDiaDiemResultVo
    {
        public KhamDoanHopDongVaDiaDiemResultVo()
        {
            KhamDoanHopDongDiaDiems = new List<KhamDoanHopDongDiaDiemResultVo>();
        }

        public int SoNguoiKham { get; set; }

        public DateTime NgayHieuLuc { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        public List<KhamDoanHopDongDiaDiemResultVo> KhamDoanHopDongDiaDiems { get; set; }
    }

    public class KhamDoanHopDongDiaDiemResultVo : GridItem
    {
        public string DiaDiem { get; set; }

        public string CongViec { get; set; }

        public DateTime? Ngay { get; set; }

        public string NgayDisplay => Ngay != null ? Ngay.GetValueOrDefault().ApplyFormatDate() : string.Empty;

        public string GhiChu { get; set; }
    }

    public class TrangThaiKhamDoanAndSoLuongResultVo
    {
        public EnumTrangThaiKhamDoan? TrangThai { get; set; }

        public int TongSoBs { get; set; }

        public int TongSoDd { get; set; }

        public int TongNvKhac { get; set; }
    }
}
