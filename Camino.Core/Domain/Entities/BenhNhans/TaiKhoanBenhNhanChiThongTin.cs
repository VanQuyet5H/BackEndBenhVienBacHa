using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class TaiKhoanBenhNhanChiThongTin : BaseEntity
    {
        public Enums.LoaiNoiDungChiTien LoaiNoiDung { get; set; }
        public string NoiDung { get; set; }

        public virtual TaiKhoanBenhNhanChi TaiKhoanBenhNhanChi { get; set; }
    }
}
