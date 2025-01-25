using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauDieuChuyenDuocPhamMapping
{
    public class YeuCauDieuChuyenDuocPhamChiTietMap : CaminoEntityTypeConfiguration<YeuCauDieuChuyenDuocPhamChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDieuChuyenDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDieuChuyenDuocPhamChiTietTable);

            builder.HasOne(m => m.YeuCauDieuChuyenDuocPham)
               .WithMany(u => u.YeuCauDieuChuyenDuocPhamChiTiets)
               .HasForeignKey(m => m.YeuCauDieuChuyenDuocPhamId);

            builder.HasOne(m => m.XuatKhoDuocPhamChiTietViTri)
              .WithMany(u => u.YeuCauDieuChuyenDuocPhamChiTiets)
              .HasForeignKey(m => m.XuatKhoDuocPhamChiTietViTriId);

            builder.HasOne(m => m.DuocPhamBenhVien)
             .WithMany(u => u.YeuCauDieuChuyenDuocPhamChiTiets)
             .HasForeignKey(m => m.DuocPhamBenhVienId);

            builder.HasOne(m => m.HopDongThauDuocPham)
            .WithMany(u => u.YeuCauDieuChuyenDuocPhamChiTiets)
            .HasForeignKey(m => m.HopDongThauDuocPhamId);

            base.Configure(builder);
        }
    }
}
