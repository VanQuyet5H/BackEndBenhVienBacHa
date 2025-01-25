
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.XetNghiems;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.DichVuXetNghiems
{
    public class DichVuXetNghiem : BaseEntity
    {
        public long NhomDichVuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int CapDichVu { get; set; }
        public long? DichVuXetNghiemChaId { get; set; }
        public string DonVi { get; set; }
        public int? SoThuTu { get; set; }
        public bool HieuLuc { get; set; }
        public string NamMin { get; set; }
        public string NamMax { get; set; }
        public string NuMin { get; set; }
        public string NuMax { get; set; }
        public string TreEmMin { get; set; }
        public string TreEmMax { get; set; }
        public string NguyHiemMax { get; set; }
        public string NguyHiemMin { get; set; }
        public string KieuDuLieu { get; set; }
        public string TreEm6Min { get; set; }
        public string TreEm6Max { get; set; }
        public string TreEm612Min { get; set; }
        public string TreEm612Max { get; set; }
        public string TreEm1218Min { get; set; }
        public string TreEm1218Max { get; set; }

        //BVHD-3901
        public long? HdppId { get; set; }
        public bool? LaChuanISO { get; set; }

        public virtual Camino.Core.Domain.Entities.HDPP.HDPP HDPP { get; set; }
        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }

        public virtual DichVuXetNghiem DichVuXetNghiemCha { get; set; }

        private ICollection<DichVuXetNghiem> _dichVuXetNghiems;
        public virtual ICollection<DichVuXetNghiem> DichVuXetNghiems
        {
            get => _dichVuXetNghiems ?? (_dichVuXetNghiems = new List<DichVuXetNghiem>());
            protected set => _dichVuXetNghiems = value;
        }


        private ICollection<DichVuKyThuatBenhVien> _dichVuKyThuatBenhViens;
        public virtual ICollection<DichVuKyThuatBenhVien> DichVuKyThuatBenhViens
        {
            get => _dichVuKyThuatBenhViens ?? (_dichVuKyThuatBenhViens = new List<DichVuKyThuatBenhVien>());
            protected set => _dichVuKyThuatBenhViens = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietChas;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTietChas
        {
            get => _ketQuaXetNghiemChiTietChas ?? (_ketQuaXetNghiemChiTietChas = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTietChas = value;
        }

        private ICollection<DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSos;
        public virtual ICollection<DichVuXetNghiemKetNoiChiSo> DichVuXetNghiemKetNoiChiSos
        {
            get => _dichVuXetNghiemKetNoiChiSos ?? (_dichVuXetNghiemKetNoiChiSos = new List<DichVuXetNghiemKetNoiChiSo>());
            protected set => _dichVuXetNghiemKetNoiChiSos = value;
        }
    }
}
