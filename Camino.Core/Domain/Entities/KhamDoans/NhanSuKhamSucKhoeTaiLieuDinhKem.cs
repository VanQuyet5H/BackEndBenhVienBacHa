using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class NhanSuKhamSucKhoeTaiLieuDinhKem : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public int LoaiTapTin { get; set; }
        public string MoTa { get; set; }

        private ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> _yeuCauNhanSuKhamSucKhoeChiTiets;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> YeuCauNhanSuKhamSucKhoeChiTiets
        {
            get => _yeuCauNhanSuKhamSucKhoeChiTiets ?? (_yeuCauNhanSuKhamSucKhoeChiTiets = new List<YeuCauNhanSuKhamSucKhoeChiTiet>());
            protected set => _yeuCauNhanSuKhamSucKhoeChiTiets = value;
        }
    }
}
