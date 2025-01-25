using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class ChuongICDMap: CaminoEntityTypeConfiguration<ChuongICD>
    {
        public override void Configure(EntityTypeBuilder<ChuongICD> builder)
        {
            builder.ToTable(MappingDefaults.ChuongICDTable);

            builder.Property(x => x.Ma).HasMaxLength(20);
            builder.Property(x => x.TenTiengAnh).HasMaxLength(250);
            builder.Property(x => x.TenTiengViet).HasMaxLength(250);

            base.Configure(builder);
        }
    }
}
