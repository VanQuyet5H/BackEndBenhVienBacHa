using Camino.Core.Domain.Entities.PhongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.PhongBenhVienMapping
{
    public class PhongBenhVienNhomGiaDichVuKyThuatMap : CaminoEntityTypeConfiguration<PhongBenhVienNhomGiaDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<PhongBenhVienNhomGiaDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.PhongBenhVienNhomGiaDichVuKyThuatTable);

                builder.HasOne(rf => rf.PhongBenhVien)
                   .WithMany(r => r.PhongBenhVienNhomGiaDichVuKyThuats)
                   .HasForeignKey(rf => rf.PhongBenhVienId);

                builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
                  .WithMany(r => r.PhongBenhVienNhomGiaDichVuKyThuats)
                  .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);

            base.Configure(builder);
        }
    }
}
