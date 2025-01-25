using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKyThuats;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoeChungDichVuDichVuKyThuat : BaseEntity
    {
        public long GoiKhamSucKhoeChungId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public decimal DonGiaUuDai { get; set; }
        public decimal DonGiaThucTe { get; set; } //BVHD-3944
        public decimal DonGiaChuaUuDai { get; set; }
        public bool GioiTinhNam { get; set; }
        public bool GioiTinhNu { get; set; }
        public bool CoMangThai { get; set; }
        public bool KhongMangThai { get; set; }
        public bool ChuaLapGiaDinh { get; set; }
        public bool DaLapGiaDinh { get; set; }
        public int? SoTuoiTu { get; set; }
        public int? SoTuoiDen { get; set; }
        public int SoLan { get; set; }

        public virtual GoiKhamSucKhoeChung GoiKhamSucKhoeChung { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }

        private ICollection<GoiKhamSucKhoeChungNoiThucHien> _goiKhamSucKhoeChungNoiThucHiens;
        public virtual ICollection<GoiKhamSucKhoeChungNoiThucHien> GoiKhamSucKhoeChungNoiThucHiens
        {
            get => _goiKhamSucKhoeChungNoiThucHiens ?? (_goiKhamSucKhoeChungNoiThucHiens = new List<GoiKhamSucKhoeChungNoiThucHien>());
            protected set => _goiKhamSucKhoeChungNoiThucHiens = value;
        }
    }
}
