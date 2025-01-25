using Camino.Core.Domain.Entities.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Messages
{
    public class QueuedSmsMap : CaminoEntityTypeConfiguration<QueuedSms>
    {
        public override void Configure(EntityTypeBuilder<QueuedSms> builder)
        {
            builder.ToTable(MappingDefaults.QueuedSmsTable);
            base.Configure(builder);
        }
    }
}
