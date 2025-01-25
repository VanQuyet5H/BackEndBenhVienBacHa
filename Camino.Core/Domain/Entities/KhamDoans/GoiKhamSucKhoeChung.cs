using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoeChung : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public bool? GoiChung { get; set; }
        public bool? GoiDichVuPhatSinh { get; set; }

        private ICollection<GoiKhamSucKhoeChungDichVuKhamBenh> _goiKhamSucKhoeChungDichVuKhamBenhs;
        public virtual ICollection<GoiKhamSucKhoeChungDichVuKhamBenh> GoiKhamSucKhoeChungDichVuKhamBenhs
        {
            get => _goiKhamSucKhoeChungDichVuKhamBenhs ?? (_goiKhamSucKhoeChungDichVuKhamBenhs = new List<GoiKhamSucKhoeChungDichVuKhamBenh>());
            protected set => _goiKhamSucKhoeChungDichVuKhamBenhs = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuDichVuKyThuat> _goiKhamSucKhoeChungDichVuDichVuKyThuats;
        public virtual ICollection<GoiKhamSucKhoeChungDichVuDichVuKyThuat> GoiKhamSucKhoeChungDichVuDichVuKyThuats
        {
            get => _goiKhamSucKhoeChungDichVuDichVuKyThuats ?? (_goiKhamSucKhoeChungDichVuDichVuKyThuats = new List<GoiKhamSucKhoeChungDichVuDichVuKyThuat>());
            protected set => _goiKhamSucKhoeChungDichVuDichVuKyThuats = value;
        }
    }
}
