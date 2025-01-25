using Camino.Core.Domain.Entities.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Messages
{
    public class QueuedEmailMap : CaminoEntityTypeConfiguration<QueuedEmail>
    {
        public override void Configure(EntityTypeBuilder<QueuedEmail> builder)
        {
            builder.ToTable(MappingDefaults.QueuedEmailTable);
            base.Configure(builder);
        }
    }
}
