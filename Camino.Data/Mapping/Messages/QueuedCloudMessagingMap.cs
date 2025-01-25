using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Messages
{
    public class QueuedCloudMessagingMap : CaminoEntityTypeConfiguration<QueuedCloudMessaging>
    {
        public override void Configure(EntityTypeBuilder<QueuedCloudMessaging> builder)
        {
            builder.ToTable(MappingDefaults.QueuedCloudMessagingTable);
            base.Configure(builder);
        }
    }
}
