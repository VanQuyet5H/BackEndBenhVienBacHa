using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoeChungDichVuKhamBenh : BaseEntity
    {
        public long GoiKhamSucKhoeChungId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public ChuyenKhoaKhamSucKhoe ChuyenKhoaKhamSucKhoe { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
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

        public virtual GoiKhamSucKhoeChung GoiKhamSucKhoeChung { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }

        private ICollection<GoiKhamSucKhoeChungNoiThucHien> _goiKhamSucKhoeChungNoiThucHiens;
        public virtual ICollection<GoiKhamSucKhoeChungNoiThucHien> GoiKhamSucKhoeChungNoiThucHiens
        {
            get => _goiKhamSucKhoeChungNoiThucHiens ?? (_goiKhamSucKhoeChungNoiThucHiens = new List<GoiKhamSucKhoeChungNoiThucHien>());
            protected set => _goiKhamSucKhoeChungNoiThucHiens = value;
        }
    }
}
