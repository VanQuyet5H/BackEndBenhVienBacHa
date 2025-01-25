using Camino.Core.Domain.Entities.DichVuKhamBenhs;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.KhoaPhongChuyenKhoas;
using Camino.Core.Domain.Entities.DichVuGiuongs;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Core.Domain.Entities.BenhVien.Khoas
{
    public class Khoa : BaseEntity
    {
        public string Ten { get; set; }

        public string Ma { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }

        private ICollection<KhoaPhongChuyenKhoa> _khoaPhongChuyenKhoas;
        public virtual ICollection<KhoaPhongChuyenKhoa> KhoaPhongChuyenKhoas
        {
            get => _khoaPhongChuyenKhoas ?? (_khoaPhongChuyenKhoas = new List<KhoaPhongChuyenKhoa>());
            protected set => _khoaPhongChuyenKhoas = value;
        }

        private ICollection<DichVuKhamBenh> _dichVuKhamBenhs;
        public virtual ICollection<DichVuKhamBenh> DichVuKhamBenhs
        {
            get => _dichVuKhamBenhs ?? (_dichVuKhamBenhs = new List<DichVuKhamBenh>());
            protected set => _dichVuKhamBenhs = value;
        }

        //private ICollection<DichVuKyThuatBenhVien> _dichVuKyThuatBenhViens;
        //public virtual ICollection<DichVuKyThuatBenhVien> DichVuKyThuatBenhViens
        //{
        //    get => _dichVuKyThuatBenhViens ?? (_dichVuKyThuatBenhViens = new List<DichVuKyThuatBenhVien>());
        //    protected set => _dichVuKyThuatBenhViens = value;
        //}

        private ICollection<DichVuGiuong> _dichVuGiuongs;
        public virtual ICollection<DichVuGiuong> DichVuGiuongs
        {
            get => _dichVuGiuongs ?? (_dichVuGiuongs = new List<DichVuGiuong>());
            protected set => _dichVuGiuongs = value;
        }

        private ICollection<ICD> _iCDs;
        public virtual ICollection<ICD> ICDs
        {
            get => _iCDs ?? (_iCDs = new List<ICD>());
            protected set => _iCDs = value;
        }
    }
}
