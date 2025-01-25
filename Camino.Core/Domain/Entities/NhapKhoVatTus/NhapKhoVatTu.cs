using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.NhapKhoVatTus
{
    public class NhapKhoVatTu : BaseEntity
    {
        public long? YeuCauNhapKhoVatTuId { get; set; }
        public long KhoId { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        public string SoChungTu { get; set; }
        public long? XuatKhoVatTuId { get; set; }
        public string TenNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }
        public long NguoiNhapId { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public DateTime? NgayHoaDon { get; set; }

        public DateTime NgayNhap { get; set; }
        public bool? DaHet { get; set; }

        public virtual YeuCauNhapKhoVatTu YeuCauNhapKhoVatTu { get; set; }
        public virtual Kho Kho { get; set; }
        public virtual YeuCauLinhVatTu YeuCauLinhVatTu { get; set; }
        public virtual XuatKhoVatTu XuatKhoVatTu { get; set; }
        public virtual NhanVien NguoiGiao { get; set; }
        public virtual NhanVien NguoiNhap { get; set; }

        private ICollection<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTiets;
        public virtual ICollection<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets
        {
            get => _nhapKhoVatTuChiTiets ?? (_nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>());
            protected set => _nhapKhoVatTuChiTiets = value;

        }
    }
}
