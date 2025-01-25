using Camino.Core.Domain.Entities.KetQuaNhomXetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.FileKetQuaCanLamSangs
{
    public class FileKetQuaCanLamSang : BaseEntity
    {
        public string Ma { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? KetQuaNhomXetNghiemId { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }

        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual KetQuaNhomXetNghiem KetQuaNhomXetNghiem { get; set; }

        //private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats { get; set; }
        //public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        //{
        //    get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
        //    protected set => _yeuCauDichVuKyThuats = value;
        //}

        //private ICollection<KetQuaNhomXetNghiem> _ketQuaNhomXetNghiems;
        //public virtual ICollection<KetQuaNhomXetNghiem> KetQuaNhomXetNghiems
        //{
        //    get => _ketQuaNhomXetNghiems ?? (_ketQuaNhomXetNghiems = new List<KetQuaNhomXetNghiem>());
        //    protected set => _ketQuaNhomXetNghiems = value;
        //}
    }
}
