using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class LoaiICDMap : CaminoEntityTypeConfiguration<LoaiICD>
    {
        public override void Configure(EntityTypeBuilder<LoaiICD> builder)
        {
            builder.ToTable(MappingDefaults.LoaiICDTable);

            builder.Property(x => x.Ma).HasMaxLength(20);
            builder.Property(x => x.TenTiengAnh).HasMaxLength(250);
            builder.Property(x => x.TenTiengViet).HasMaxLength(250);

            builder.HasOne(rf => rf.NhomICD)
                .WithMany(r => r.LoaiICDs)
                .HasForeignKey(rf => rf.NhomICDId);

            base.Configure(builder);
        }
    }
}
