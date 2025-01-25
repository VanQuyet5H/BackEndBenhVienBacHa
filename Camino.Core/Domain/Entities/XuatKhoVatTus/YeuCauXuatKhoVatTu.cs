using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhaThaus;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.XuatKhoVatTus
{
    public class YeuCauXuatKhoVatTu : BaseEntity
    {
        public long KhoXuatId { get; set; }
        public long NguoiXuatId { get; set; }
        public long? NguoiNhanId { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public DateTime NgayXuat { get; set; }
        public string LyDoXuatKho { get; set; }
        public bool? TraNCC { get; set; }
        public long? NhaThauId { get; set; }
        public string SoChungTu { get; set; }
        public virtual NhaThau NhaThau { get; set; }
        public virtual Kho KhoXuat { get; set; }
        public virtual NhanVien NguoiXuat { get; set; }
        public virtual NhanVien NguoiNhan { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }
        private ICollection<YeuCauXuatKhoVatTuChiTiet> _yeuCauXuatKhoVatTuChiTiets;
        public virtual ICollection<YeuCauXuatKhoVatTuChiTiet> YeuCauXuatKhoVatTuChiTiets
        {
            get => _yeuCauXuatKhoVatTuChiTiets ?? (_yeuCauXuatKhoVatTuChiTiets = new List<YeuCauXuatKhoVatTuChiTiet>());
            protected set => _yeuCauXuatKhoVatTuChiTiets = value;
        }
    }
}
