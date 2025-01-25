using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.AuditColumn
{
    public class AuditColumnMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.Audit.AuditColumn>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.Audit.AuditColumn> builder)
        {
            builder.ToTable(MappingDefaults.AuditColumn);

            builder.HasOne(rf => rf.AuditTable)
                .WithMany(r => r.AuditColumns)
                .HasForeignKey(rf => rf.AuditTableId);

            base.Configure(builder);
        }
    }
}