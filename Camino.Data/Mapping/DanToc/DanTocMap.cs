using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.DanToc
{
    public class DanTocMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DanTocs.DanToc>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DanTocs.DanToc> builder)
        {
            builder.ToTable(MappingDefaults.DanTocTable);
            builder.HasOne(rf => rf.QuocGia)
               .WithMany(r => r.DanTocs)
               .HasForeignKey(rf => rf.QuocGiaId);
            base.Configure(builder);
        }
    }
}
