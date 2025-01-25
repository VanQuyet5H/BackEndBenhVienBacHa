using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoeDichVuKhamBenh : BaseEntity
    {
        public long GoiKhamSucKhoeId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
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

        public virtual GoiKhamSucKhoe GoiKhamSucKhoe { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }

        private ICollection<GoiKhamSucKhoeNoiThucHien> _goiKhamSucKhoeNoiThucHiens;
        public virtual ICollection<GoiKhamSucKhoeNoiThucHien> GoiKhamSucKhoeNoiThucHiens
        {
            get => _goiKhamSucKhoeNoiThucHiens ?? (_goiKhamSucKhoeNoiThucHiens = new List<GoiKhamSucKhoeNoiThucHien>());
            protected set => _goiKhamSucKhoeNoiThucHiens = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> _goiKhamSucKhoeChungDichVuKhamBenhNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> GoiKhamSucKhoeChungDichVuKhamBenhNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens ?? (_goiKhamSucKhoeChungDichVuKhamBenhNhanViens = new List<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens = value;
        }
    }
}
