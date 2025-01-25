using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.NhapKhoQuaTangs
{
    public class NhapKhoQuaTang : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; private set; }
        public string SoChungTu { get; private set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public string TenNguoiGiao { get; private set; }
        public long? NguoiGiaoId { get; set; }
        public long NguoiNhapId { get; set; }
        public DateTime NgayNhap { get; set; }
        public bool? DaHet { get; set; }

        public virtual NhanVien NguoiNhap { get; set; }
        public virtual NhanVien NguoiGiao { get; set; }

        private ICollection<NhapKhoQuaTangChiTiet> _nhapKhoQuaTangChiTiets { get; set; }
        public virtual ICollection<NhapKhoQuaTangChiTiet> NhapKhoQuaTangChiTiets
        {
            get => _nhapKhoQuaTangChiTiets ?? (_nhapKhoQuaTangChiTiets = new List<NhapKhoQuaTangChiTiet>());
            protected set => _nhapKhoQuaTangChiTiets = value;
        }
    }
}
