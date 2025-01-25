using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoe : BaseEntity
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }

        //BVHD-3250
        public bool? GoiChung { get; set; }

        //BVHD-3668
        public bool? GoiDichVuPhatSinh { get; set; }

        public virtual HopDongKhamSucKhoe HopDongKhamSucKhoe { get; set; }

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeNhanViens
        {
            get => _hopDongKhamSucKhoeNhanViens ?? (_hopDongKhamSucKhoeNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeNhanViens = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<GoiKhamSucKhoeDichVuKhamBenh> _goiKhamSucKhoeDichVuKhamBenhs;
        public virtual ICollection<GoiKhamSucKhoeDichVuKhamBenh> GoiKhamSucKhoeDichVuKhamBenhs
        {
            get => _goiKhamSucKhoeDichVuKhamBenhs ?? (_goiKhamSucKhoeDichVuKhamBenhs = new List<GoiKhamSucKhoeDichVuKhamBenh>());
            protected set => _goiKhamSucKhoeDichVuKhamBenhs = value;
        }

        private ICollection<GoiKhamSucKhoeDichVuDichVuKyThuat> _goiKhamSucKhoeDichVuDichVuKyThuats;
        public virtual ICollection<GoiKhamSucKhoeDichVuDichVuKyThuat> GoiKhamSucKhoeDichVuDichVuKyThuats
        {
            get => _goiKhamSucKhoeDichVuDichVuKyThuats ?? (_goiKhamSucKhoeDichVuDichVuKyThuats = new List<GoiKhamSucKhoeDichVuDichVuKyThuat>());
            protected set => _goiKhamSucKhoeDichVuDichVuKyThuats = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> _goiKhamSucKhoeChungDichVuKhamBenhNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> GoiKhamSucKhoeChungDichVuKhamBenhNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens ?? (_goiKhamSucKhoeChungDichVuKhamBenhNhanViens = new List<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> _goiKhamSucKhoeChungDichVuKyThuatNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> GoiKhamSucKhoeChungDichVuKyThuatNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKyThuatNhanViens ?? (_goiKhamSucKhoeChungDichVuKyThuatNhanViens = new List<GoiKhamSucKhoeChungDichVuKyThuatNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKyThuatNhanViens = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhKhamSucKhoeDichVuPhatSinhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhKhamSucKhoeDichVuPhatSinhs
        {
            get => _yeuCauKhamBenhKhamSucKhoeDichVuPhatSinhs ?? (_yeuCauKhamBenhKhamSucKhoeDichVuPhatSinhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhKhamSucKhoeDichVuPhatSinhs = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatKhamSucKhoeDichVuPhatSinhs;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuatKhamSucKhoeDichVuPhatSinhs
        {
            get => _yeuCauDichVuKyThuatKhamSucKhoeDichVuPhatSinhs ?? (_yeuCauDichVuKyThuatKhamSucKhoeDichVuPhatSinhs = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuatKhamSucKhoeDichVuPhatSinhs = value;
        }
    }
}
