using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class TaiKhoanBenhNhanHuyDichVu : BaseEntity
    {
        public long TaiKhoanBenhNhanId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string LyDoHuy { get; set; }
        public DateTime NgayHuy { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }

        public virtual TaiKhoanBenhNhan TaiKhoanBenhNhan { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }

        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
    }
}
