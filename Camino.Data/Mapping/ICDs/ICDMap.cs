using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class ICDMap: CaminoEntityTypeConfiguration<ICD>
    {
        public override void Configure(EntityTypeBuilder<ICD> builder)
        {
            builder.ToTable(MappingDefaults.ICDTable);

            builder.Property(x => x.Ma).HasMaxLength(20);
            builder.Property(x => x.TenTiengAnh).HasMaxLength(250);
            builder.Property(x => x.TenTiengViet).HasMaxLength(250);

            builder.HasOne(rf => rf.LoaiICD)
                .WithMany(r => r.ICDs)
                .HasForeignKey(rf => rf.LoaiICDId);

            builder.HasOne(rf => rf.Khoa)
               .WithMany(r => r.ICDs)
               .HasForeignKey(rf => rf.KhoaId);

            base.Configure(builder);
        }
    }
}
