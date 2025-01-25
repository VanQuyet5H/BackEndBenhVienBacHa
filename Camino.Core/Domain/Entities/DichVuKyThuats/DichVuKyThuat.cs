using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class DichVuKyThuat : BaseEntity
    {
        public string TenChung { get; set; }
        public string TenTiengAnh { get; set; }
        public string MaChung { get; set; }
        public string Ma4350 { get; set; }
        public string MaGia { get; set; }
        public string TenGia { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi NhomChiPhi { get; set; }
        public long NhomDichVuKyThuatId { get; set; }
        public Enums.LoaiPhauThuatThuThuat? LoaiPhauThuatThuThuat { get; set; }
        public bool? NgoaiQuyDinhBHYT { get; set; }
        public string MoTa { get; set; }

        public virtual NhomDichVuKyThuat NhomDichVuKyThuat { get; set; }

        private ICollection<DichVuKyThuatThongTinGia> _dichVuKyThuatThongTinGias { get; set; }
        public virtual ICollection<DichVuKyThuatThongTinGia> DichVuKyThuatThongTinGias
        {
            get => _dichVuKyThuatThongTinGias ?? (_dichVuKyThuatThongTinGias = new List<DichVuKyThuatThongTinGia>());
            protected set => _dichVuKyThuatThongTinGias = value;
        }

        private ICollection<DichVuKyThuatBenhVien> _dichVuKyThuatBenhViens;
        public virtual ICollection<DichVuKyThuatBenhVien> DichVuKyThuatBenhViens
        {
            get => _dichVuKyThuatBenhViens ?? (_dichVuKyThuatBenhViens = new List<DichVuKyThuatBenhVien>());
            protected set => _dichVuKyThuatBenhViens = value;
        }
    }
}
