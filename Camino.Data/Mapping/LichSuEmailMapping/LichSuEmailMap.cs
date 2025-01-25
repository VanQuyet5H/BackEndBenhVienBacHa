using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.LichSuEmailMapping
{
    public class LichSuEmailMap: CaminoEntityTypeConfiguration<LichSuEmail>
    {
        public override void Configure(EntityTypeBuilder<LichSuEmail> builder)
        {
            builder.ToTable(MappingDefaults.LichSuEmailTable);
            builder.Property(x => x.TapTinDinhKem).HasMaxLength(200);
            builder.Property(x => x.TieuDe).HasMaxLength(100);
            builder.Property(x => x.NoiDung).HasMaxLength(2000);
            builder.Property(x => x.GoiDen).HasMaxLength(50);
            base.Configure(builder);
        }
    }
}
