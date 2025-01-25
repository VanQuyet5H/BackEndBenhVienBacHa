using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class ChuanDoanLienKetICDMap : CaminoEntityTypeConfiguration<ChuanDoanLienKetICD>
    {
        public override void Configure(EntityTypeBuilder<ChuanDoanLienKetICD> builder)
        {
            builder.ToTable(MappingDefaults.ChuanDoanLienKetICDTable);

            builder.HasOne(rf => rf.ICD)
                .WithMany(r => r.ChuanDoanLienKetICDs)
                .HasForeignKey(rf => rf.ICDId);
            builder.HasOne(rf => rf.ChuanDoan)
                .WithMany(r => r.ChuanDoanLienKetICDs)
                .HasForeignKey(rf => rf.ChuanDoanId);

            base.Configure(builder);
        }
    }
}
