using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class YeuCauHoanTraVatTuThuocPTTTVo : GridItem
    {
        public YeuCauHoanTraVatTuThuocPTTTVo()
        {
            YeuCauDuocPhamVatTuBenhViens = new List<YeuCauHoanTraVatTuThuocChiTietPTTTVo>();
        }

        public long VatTuThuocBenhVienId { get; set; }
        //public bool LaVatTuBHYT { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long NhanVienYeuCauId { get; set; }
        //public double? SoLuongTra { get; set; }
        //public double SoLuong { get; set; }
        //public double SoLuongDaTra { get; set; }
        public List<YeuCauHoanTraVatTuThuocChiTietPTTTVo> YeuCauDuocPhamVatTuBenhViens { get; set; }
    }

    public class YeuCauHoanTraVatTuThuocChiTietPTTTVo : GridItem
    {
        public double? SoLuongTra { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaTra { get; set; }
        public EnumNhomGoiDichVu NhomYeuCauId { get; set; }

    }
}