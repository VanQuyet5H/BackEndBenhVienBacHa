using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.ChucDanh
{
    public class ChucDanhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.ChucDanhs.ChucDanh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.ChucDanhs.ChucDanh> builder)
        {
            builder.ToTable(MappingDefaults.ChucDanhTable);
            builder.HasOne(rf => rf.NhomChucDanh)
                .WithMany(r => r.ChucDanhs)
                .HasForeignKey(rf => rf.NhomChucDanhId);
            base.Configure(builder);
        }
    }
}
