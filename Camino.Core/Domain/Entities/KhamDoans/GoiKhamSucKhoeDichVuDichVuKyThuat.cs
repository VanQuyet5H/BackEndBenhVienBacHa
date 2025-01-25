using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoeDichVuDichVuKyThuat : BaseEntity
    {
        public long GoiKhamSucKhoeId { get; set; }
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

        public virtual GoiKhamSucKhoe GoiKhamSucKhoe { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }

        private ICollection<GoiKhamSucKhoeNoiThucHien> _goiKhamSucKhoeNoiThucHiens;
        public virtual ICollection<GoiKhamSucKhoeNoiThucHien> GoiKhamSucKhoeNoiThucHiens
        {
            get => _goiKhamSucKhoeNoiThucHiens ?? (_goiKhamSucKhoeNoiThucHiens = new List<GoiKhamSucKhoeNoiThucHien>());
            protected set => _goiKhamSucKhoeNoiThucHiens = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> _goiKhamSucKhoeChungDichVuKyThuatNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> GoiKhamSucKhoeChungDichVuKyThuatNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKyThuatNhanViens ?? (_goiKhamSucKhoeChungDichVuKyThuatNhanViens = new List<GoiKhamSucKhoeChungDichVuKyThuatNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKyThuatNhanViens = value;
        }
    }
}
