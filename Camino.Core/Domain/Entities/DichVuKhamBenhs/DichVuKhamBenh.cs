using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;

namespace Camino.Core.Domain.Entities.DichVuKhamBenhs
{
    public class DichVuKhamBenh : BaseEntity
    {
        public string MaChung { get; set; }
        public string MaTT37 { get; set; }
        public string TenChung { get; set; }
        public long KhoaId { get; set; }
        public Enums.HangBenhVien? HangBenhVien { get; set; }
        public string MoTa { get; set; }
        public virtual Khoa Khoa { get; set; }
        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        //{
        //    get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhs = value;
        //}
        private ICollection<DichVuKhamBenhThongTinGia> _dichVuKhamBenhThongTinGias { get; set; }
        public virtual ICollection<DichVuKhamBenhThongTinGia> DichVuKhamBenhThongTinGias
        {
            get => _dichVuKhamBenhThongTinGias ?? (_dichVuKhamBenhThongTinGias = new List<DichVuKhamBenhThongTinGia>());
            protected set => _dichVuKhamBenhThongTinGias = value;
        }

        private ICollection<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhViens;
        public virtual ICollection<DichVuKhamBenhBenhVien> DichVuKhamBenhBenhViens
        {
            get => _dichVuKhamBenhBenhViens ?? (_dichVuKhamBenhBenhViens = new List<DichVuKhamBenhBenhVien>());
            protected set => _dichVuKhamBenhBenhViens = value;
        }
    }
}
