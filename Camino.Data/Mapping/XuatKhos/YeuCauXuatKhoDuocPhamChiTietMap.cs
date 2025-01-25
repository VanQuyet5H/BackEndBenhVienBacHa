using Camino.Core.Domain.Entities.XuatKhos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhos
{
    public class YeuCauXuatKhoDuocPhamChiTietMap : CaminoEntityTypeConfiguration<YeuCauXuatKhoDuocPhamChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauXuatKhoDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauXuatKhoDuocPhamChiTietTable);

            builder.HasOne(m => m.YeuCauXuatKhoDuocPham)
                .WithMany(u => u.YeuCauXuatKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.YeuCauXuatKhoDuocPhamId);

            builder.HasOne(m => m.XuatKhoDuocPhamChiTietViTri)
                .WithMany(u => u.YeuCauXuatKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.XuatKhoDuocPhamChiTietViTriId);

            builder.HasOne(m => m.DuocPhamBenhVien)
                .WithMany(u => u.YeuCauXuatKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.DuocPhamBenhVienId);

            builder.HasOne(m => m.HopDongThauDuocPham)
              .WithMany(u => u.YeuCauXuatKhoDuocPhamChiTiets)
              .HasForeignKey(m => m.HopDongThauDuocPhamId);

            builder.HasOne(m => m.MayXetNghiem)
                .WithMany(u => u.YeuCauXuatKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.MayXetNghiemId);

            base.Configure(builder);
        }
    }
}
