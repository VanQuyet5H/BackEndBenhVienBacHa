using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Helpers;

namespace Camino.Api.Models.XuatKhos
{
    public class XuatKhoDuocPhamKhacViewModel : BaseViewModel
    {
        public XuatKhoDuocPhamKhacViewModel()
        {
            YeuCauXuatKhoDuocPhamChiTiets = new List<XuatKhoKhacDuocPhamChiTietVo>();
            YeuCauXuatKhoDuocPhamChiTietHienThis = new List<YeuCauXuatKhoDuocPhamGridVo>();

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
        public long? NhapKhoDuocPhamId { get; set; }
        public string SoChungTu { get; set; }


        public Enums.EnumLoaiKhoDuocPham? LoaiKho { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public string LyDoXuatKho { get; set; }
        public DateTime? NgayXuat { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public List<XuatKhoKhacDuocPhamChiTietVo> YeuCauXuatKhoDuocPhamChiTiets { get; set; }
        public List<YeuCauXuatKhoDuocPhamGridVo> YeuCauXuatKhoDuocPhamChiTietHienThis { get; set; }
    }
}
