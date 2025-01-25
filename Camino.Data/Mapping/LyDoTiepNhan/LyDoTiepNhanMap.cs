using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.LyDoTiepNhan
{
    public class LyDoTiepNhanMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan> builder)
        {
            builder.ToTable(MappingDefaults.LyDoTiepNhanTable);

            builder.HasOne(rf => rf.LyDoTiepNhanCha)
               .WithMany(r => r.LyDoTiepNhans)
               .HasForeignKey(rf => rf.LyDoTiepNhanChaId);

            base.Configure(builder);
        }
    }
}
