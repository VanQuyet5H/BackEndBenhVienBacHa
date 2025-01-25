using System.Collections.Generic;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.MauMayXetNghiemChiSos;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;

namespace Camino.Core.Domain.Entities.MauMayXetNghiems
{
    public class MauMayXetNghiem : BaseEntity
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenTiengAnh { get; set; }

        public string NhaSanXuat { get; set; }

        public long? NhomDichVuBenhVienId { get; set; }

        public string MoTa { get; set; }

        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }

        private ICollection<MayXetNghiem> _mayXetNghiems;
        public virtual ICollection<MayXetNghiem> MayXetNghiems
        {
            get => _mayXetNghiems ?? (_mayXetNghiems = new List<MayXetNghiem>());
            protected set => _mayXetNghiems = value;
        }

        private ICollection<MauMayXetNghiemChiSo> _mauMayXetNghiemChiSos;
        public virtual ICollection<MauMayXetNghiemChiSo> MauMayXetNghiemChiSos
        {
            get => _mauMayXetNghiemChiSos ?? (_mauMayXetNghiemChiSos = new List<MauMayXetNghiemChiSo>());
            protected set => _mauMayXetNghiemChiSos = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }

        private ICollection<DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSos;
        public virtual ICollection<DichVuXetNghiemKetNoiChiSo> DichVuXetNghiemKetNoiChiSos
        {
            get => _dichVuXetNghiemKetNoiChiSos ?? (_dichVuXetNghiemKetNoiChiSos = new List<DichVuXetNghiemKetNoiChiSo>());
            protected set => _dichVuXetNghiemKetNoiChiSos = value;
        }
    }
}
