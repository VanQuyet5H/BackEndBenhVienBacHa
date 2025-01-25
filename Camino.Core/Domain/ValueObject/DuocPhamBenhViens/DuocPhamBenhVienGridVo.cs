using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DuocPhamBenhViens
{
    public class DuocPhamBenhVienGridVo : GridItem
    {
        public bool HieuLuc { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }

        public string Ten { get; set; }
        public string SoDangKy { get; set; }
        public string MaHoatChat { get; set; }
        public string Ma { get; set; }
        public string HoatChat { get; set; }
        public string TenDuongDung { get; set; }
        public string QuyCach { get; set; }
        public string TenDonViTinh { get; set; }
        public string HamLuong { get; set; }
        public string TenLoaiThuocHoacHoatChat { get; set; }
        public string PhanNhom { get; set; }
    }
    public class NhomDichVuBenhVienPhanNhomTreeViewVo : LookupItemVo
    {
        public NhomDichVuBenhVienPhanNhomTreeViewVo()
        {
            Items = new List<NhomDichVuBenhVienPhanNhomTreeViewVo>();
        }

        public bool IsDisabled { get; set; }
        public int Level { get; set; }
        public long? ParentId { get; set; }
        public List<NhomDichVuBenhVienPhanNhomTreeViewVo> Items { get; set; }
    }

    public class PhanLoaiThuocTheoQuanLyLookupItemVo
    {
        public PhanLoaiThuocTheoQuanLyLookupItemVo()
        {
            LookupItemVo = new List<LookupItemVo>();
        }
        public List<LookupItemVo> LookupItemVo { get; set; }
    }
}