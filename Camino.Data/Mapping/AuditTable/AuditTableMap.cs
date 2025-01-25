using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.AuditTable
{
    public class AuditTableMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.Audit.AuditTable>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.Audit.AuditTable> builder)
        {
            builder.ToTable(MappingDefaults.AuditTable);
            base.Configure(builder);
        }
    }
}