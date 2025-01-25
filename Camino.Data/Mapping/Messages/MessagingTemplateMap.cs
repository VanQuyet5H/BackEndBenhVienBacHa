using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Messages
{
    public class MessagingTemplateMap : CaminoEntityTypeConfiguration<MessagingTemplate>
    {
        public override void Configure(EntityTypeBuilder<MessagingTemplate> builder)
        {
            builder.ToTable(MappingDefaults.MessagingTemplateTable);
            base.Configure(builder);
        }
    }
}
