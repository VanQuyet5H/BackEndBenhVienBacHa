using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class SoDoGiuongBenhTheoPhongKhamVo
    {
        public SoDoGiuongBenhTheoPhongKhamVo()
        {
            GiuongBenhs = new List<GiuongBenhTheoPhongBenhVienVo>();
        }
        public long PhongBenhVienId { get; set; }
        public string MaPhongBenhVien { get; set; }
        public string TenPhongBenhVien { get; set; }
        public string DisplayName { get; set; }
        public List<GiuongBenhTheoPhongBenhVienVo> GiuongBenhs { get; set; }
    }

    public class GiuongBenhTheoPhongBenhVienVo
    {
        public long GiuongBenhId { get; set; }
        public string TenGiuong { get; set; }
        public string DisplayName { get; set; }
        public int SoNguoiHienTai { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class LookupItemGiuongBenhVo : LookupItemVo
    {
        public int SoBenhNhanHienTai { get; set; }
    }
}
