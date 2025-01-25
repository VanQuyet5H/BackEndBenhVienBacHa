using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatTable);

            builder.HasOne(rf => rf.NhomDichVuKyThuat)
                .WithMany(r => r.DichVuKyThuats)
                .HasForeignKey(rf => rf.NhomDichVuKyThuatId);

            base.Configure(builder);
        }
    }
}
