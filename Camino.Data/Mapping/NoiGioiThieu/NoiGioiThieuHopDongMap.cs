using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDong>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDong> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongTable);

            builder.HasOne(rf => rf.NoiGioiThieu)
                .WithMany(r => r.NoiGioiThieuHopDongs)
                .HasForeignKey(rf => rf.NoiGioiThieuId);

            base.Configure(builder);
        }
    }
}
