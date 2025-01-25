using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.NhapKhoMarketting
{
    public class QuaTangMarketingGridVo : GridItem
    {
        public string Ten { get; set; }
        public string DonViTinh { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public string HieuLucDisplay => HieuLuc ? "Đang sử dụng" : "Ngừng sử dụng";
    }

    public class NhapKhoQuaTangMarketingGridVo : GridItem
    {
        public string SoPhieu { get; set; }
        public long? NguoiNhapId { get; set; }
        public string NhanVienNhap { get; set; }

        public DateTime? NgayNhap { get; set; }
        public string NgayNhapDisplay { get; set; }
        public string GhiChu { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SearchString { get; set; }
        // 
        public string SoChungTu { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public string LoaiNguoiGiaoString => LoaiNguoiGiao.GetDescription();
        public string TenNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }

    }

    public class NhapKhoQuaTangMarketingChiTietGridVo : GridItem
    {
        public string Ten { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuongNhap { get; set; }
        public long KhoNhapId { get; set; }
        public string TenKhoNhap { get; set; }
    }
}
