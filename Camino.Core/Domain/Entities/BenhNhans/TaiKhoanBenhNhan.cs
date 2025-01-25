using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class TaiKhoanBenhNhan : BaseEntity
    {
        public virtual BenhNhan BenhNhan { get; set; }
        public decimal SoDuTaiKhoan { get; set; }

        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }

        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }

        private ICollection<TaiKhoanBenhNhanHuyDichVu> _taiKhoanBenhNhanHuyDichVus;
        public virtual ICollection<TaiKhoanBenhNhanHuyDichVu> TaiKhoanBenhNhanHuyDichVus
        {
            get => _taiKhoanBenhNhanHuyDichVus ?? (_taiKhoanBenhNhanHuyDichVus = new List<TaiKhoanBenhNhanHuyDichVu>());
            protected set => _taiKhoanBenhNhanHuyDichVus = value;
        }
    }
}
