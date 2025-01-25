using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.XuatKhoVatTus
{
    public class XuatKhoVatTu : BaseEntity
    {
        public long KhoXuatId { get; set; }
        public long? KhoNhapId { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public Enums.EnumLoaiXuatKho LoaiXuatKho { get; set; }
        public string LyDoXuatKho { get; set; }
        public string TenNguoiNhan { get; set; }
        public long? NguoiNhanId { get; set; }
        public long NguoiXuatId { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiNhan { get; set; }
        public DateTime NgayXuat { get; set; }
        public virtual Kho KhoVatTuXuat { get; set; }
        public virtual Kho KhoVatTuNhap { get; set; }

        public virtual YeuCauLinhVatTu YeuCauLinhVatTu { get; set; }
        public virtual NhanVien NguoiNhan { get; set; }
        public virtual NhanVien NguoiXuat { get; set; }

        public bool? TraNCC { get; set; }

        public long? NhaThauId { get; set; }
        public string SoChungTu { get; set; }
        public virtual NhaThau NhaThau { get; set; }

        private ICollection<NhapKhoVatTu> _nhapKhoVatTus;

        public virtual ICollection<NhapKhoVatTu> NhapKhoVatTus
        {
            get => _nhapKhoVatTus ?? (_nhapKhoVatTus = new List<NhapKhoVatTu>());
            protected set => _nhapKhoVatTus = value;
        }

        private ICollection<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTiets;

        public virtual ICollection<XuatKhoVatTuChiTiet> XuatKhoVatTuChiTiets
        {
            get => _xuatKhoVatTuChiTiets ?? (_xuatKhoVatTuChiTiets = new List<XuatKhoVatTuChiTiet>());
            protected set => _xuatKhoVatTuChiTiets = value;
        }

        #region clone
        public XuatKhoVatTu Clone()
        {
            return (XuatKhoVatTu)this.MemberwiseClone();
        }
        #endregion
    }
}
