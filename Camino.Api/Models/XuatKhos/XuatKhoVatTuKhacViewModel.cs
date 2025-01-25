using Camino.Core.Domain.ValueObject.XuatKhos;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.XuatKhos
{
    public class XuatKhoVatTuKhacViewModel : BaseViewModel
    {
        public XuatKhoVatTuKhacViewModel()
        {
            YeuCauXuatKhoVatTuChiTiets = new List<XuatKhoKhacVatTuChiTietVo>();
            YeuCauXuatKhoVatTuChiTietHienThis = new List<YeuCauXuatKhoVatTuGridVo>();

        }
        public long? KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long? NguoiXuatId { get; set; }
        public string TenNguoiXuat { get; set; }
        public long? NguoiNhanId { get; set; }
        public string TenNguoiNhan { get; set; }
        public bool LaLuuDuyet { get; set; }
        public bool? TraNCC { get; set; }
        public string TenNhaThau { get; set; }
        public long? NhaThauId { get; set; }
        public long? NhapKhoVatTuId { get; set; }
        public string SoChungTu { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public string LyDoXuatKho { get; set; }
        public DateTime? NgayXuat { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public List<XuatKhoKhacVatTuChiTietVo> YeuCauXuatKhoVatTuChiTiets { get; set; }
        public List<YeuCauXuatKhoVatTuGridVo> YeuCauXuatKhoVatTuChiTietHienThis { get; set; }
    }
}
