using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.GachNos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GachNoMapping
{
    public class AuditGachNoMap : CaminoEntityTypeConfiguration<AuditGachNo>
    {
        public override void Configure(EntityTypeBuilder<AuditGachNo> builder)
        {
            builder.ToTable(MappingDefaults.AuditGachNoTable);
            builder.Property(x => x.TableName).IsRequired();
            builder.Property(x => x.Action).IsRequired();

            builder.HasOne(x => x.NhanVienThucHien)
                .WithMany(y => y.AuditGachNos)
                .HasForeignKey(x => x.CreatedById);

            base.Configure(builder);
        }
    }
}
