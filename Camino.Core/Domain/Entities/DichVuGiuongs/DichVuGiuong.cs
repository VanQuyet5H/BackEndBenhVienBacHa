using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.DichVuGiuongThongTinGias;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;

namespace Camino.Core.Domain.Entities.DichVuGiuongs
{
    public class DichVuGiuong : BaseEntity
    {
        public string MaChung { get; set; }
        public string MaTT37 { get; set; }
        public string TenChung { get; set; }
        public long? KhoaId { get; set; }
        public Enums.HangBenhVien HangBenhVien { get; set; }
        public bool? NgoaiQuyDinhBHYT { get; set; }

        public string MoTa { get; set; }
        public virtual Khoa Khoa { get; set; }
        //public virtual DichVuGiuongThongTinGia DichVuGiuongThongTinGia { get; set; }

        private ICollection<DichVuGiuongThongTinGia> _dichVuGiuongThongTinGias;

        public virtual ICollection<DichVuGiuongThongTinGia> DichVuGiuongThongTinGias
        {
            get => _dichVuGiuongThongTinGias ?? (_dichVuGiuongThongTinGias = new List<DichVuGiuongThongTinGia>());
            protected set => _dichVuGiuongThongTinGias = value;
        }


        private ICollection<DichVuGiuongBenhVien> _dichVuGiuongBenhViens;

        public virtual ICollection<DichVuGiuongBenhVien> DichVuGiuongBenhViens
        {
            get => _dichVuGiuongBenhViens ?? (_dichVuGiuongBenhViens = new List<DichVuGiuongBenhVien>());
            protected set => _dichVuGiuongBenhViens = value;
        }
    }
}
