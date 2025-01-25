using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.HamGuiHoSoWatchings
{
   public class HamGuiHoSoWatchingMap : CaminoEntityTypeConfiguration<HamGuiHoSoWatching>
    {
        public override void Configure(EntityTypeBuilder<HamGuiHoSoWatching> builder)
        {
            builder.ToTable(MappingDefaults.HamGuiHoSoWatchingTable);
            base.Configure(builder);
        }
    }
}
