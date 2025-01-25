using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class NoiTruChiDinhPhaThuocTruyen : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruBenhAnId { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public int? SoLanTrenNgay { get; set; }
        public double? CachGioTruyen { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }

        private ICollection<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhams;
        public virtual ICollection<NoiTruChiDinhDuocPham> NoiTruChiDinhDuocPhams
        {
            get => _noiTruChiDinhDuocPhams ?? (_noiTruChiDinhDuocPhams = new List<NoiTruChiDinhDuocPham>());
            protected set => _noiTruChiDinhDuocPhams = value;
        }
    }
}
