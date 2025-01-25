using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class NhomICDMap : CaminoEntityTypeConfiguration<NhomICD>
    {
        public override void Configure(EntityTypeBuilder<NhomICD> builder)
        {
            builder.ToTable(MappingDefaults.NhomICDTable);

            builder.Property(x => x.Ma).HasMaxLength(20);
            builder.Property(x => x.TenTiengAnh).HasMaxLength(250);
            builder.Property(x => x.TenTiengViet).HasMaxLength(250);

            builder.HasOne(rf => rf.ChuongICD)
                .WithMany(r => r.NhomICDs)
                .HasForeignKey(rf => rf.ChuongICDId);

            base.Configure(builder);
        }
    }
}
