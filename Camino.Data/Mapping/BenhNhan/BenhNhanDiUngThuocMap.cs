using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class BenhNhanDiUngThuocMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc> builder)
        {
            builder.ToTable(MappingDefaults.BenhNhanDiUngThuocTable);

                builder.HasOne(m => m.BenhNhan)
                    .WithMany(u => u.BenhNhanDiUngThuocs)
                    .HasForeignKey(m => m.BenhNhanId)
                    .IsRequired();

            base.Configure(builder);
        }

    }
}
