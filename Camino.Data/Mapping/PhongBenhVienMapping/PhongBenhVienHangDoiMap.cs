using Camino.Core.Domain.Entities.PhongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.PhongBenhVienMapping
{
    public class PhongBenhVienHangDoiMap : CaminoEntityTypeConfiguration<PhongBenhVienHangDoi>
    {
        public override void Configure(EntityTypeBuilder<PhongBenhVienHangDoi> builder)
        {
            builder.ToTable(MappingDefaults.PhongBenhVienHangDoiTable);
            builder.HasOne(m => m.PhongBenhVien)
                .WithMany(u => u.PhongBenhVienHangDois)
                .HasForeignKey(m => m.PhongBenhVienId)
                .IsRequired();
            builder.HasOne(m => m.YeuCauTiepNhan)
                .WithMany(u => u.PhongBenhVienHangDois)
                .HasForeignKey(m => m.YeuCauTiepNhanId)
                .IsRequired();

            builder.HasOne(m => m.YeuCauKhamBenh)
                .WithMany(u => u.PhongBenhVienHangDois)
                .HasForeignKey(m => m.YeuCauKhamBenhId);

            builder.HasOne(m => m.YeuCauDichVuKyThuat)
             .WithMany(u => u.PhongBenhVienHangDois)
             .HasForeignKey(m => m.YeuCauDichVuKyThuatId);
        }
    }
}