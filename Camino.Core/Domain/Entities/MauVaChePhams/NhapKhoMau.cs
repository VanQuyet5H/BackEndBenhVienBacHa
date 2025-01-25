using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhaThaus;

namespace Camino.Core.Domain.Entities.MauVaChePhams
{
    public class NhapKhoMau : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public string SoHoaDon { get; set; }
        public string KyHieuHoaDon { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }
        public string TenNguoiGiao { get; set; }
        public long? NhaThauId { get; set; }
        public DateTime NgayNhap { get; set; }
        public long NguoiNhapId { get; set; }
        public bool? DuocKeToanDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string GhiChu { get; set; }

        public virtual NhanVien NguoiGiao { get; set; }
        public virtual NhaThau NhaThau { get; set; }
        public virtual NhanVien NguoiNhap { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }

        private ICollection<NhapKhoMauChiTiet> _nhapKhoMauChiTiets;
        public virtual ICollection<NhapKhoMauChiTiet> NhapKhoMauChiTiets
        {
            get => _nhapKhoMauChiTiets ?? (_nhapKhoMauChiTiets = new List<NhapKhoMauChiTiet>());
            protected set => _nhapKhoMauChiTiets = value;
        }
    }
}
