using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.QuayThuoc
{
   public class DanhSachChoXuatThuocVO
    {
        public DanhSachChoXuatThuocVO()
        {
            ThuocBHYT = new List<ThongTinDuocPhamQuayThuocVo>();
            ThuocKhongBHYT = new List<ThongTinDuocPhamQuayThuocVo>();
        }
        public long Id { get; set; }
        public List<ThongTinDuocPhamQuayThuocVo> ThuocBHYT { get; set; }
        public List<ThongTinDuocPhamQuayThuocVo> ThuocKhongBHYT { get; set; }

        public class TinhQuan
        {
            public long TinhThanh { get; set; }
            public long QuanHuyen { get; set; }

        }
       
    }
}
