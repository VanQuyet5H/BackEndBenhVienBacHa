using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruHoSoKhacFileDinhKemMap : CaminoEntityTypeConfiguration<NoiTruHoSoKhacFileDinhKem>
    {
        public override void Configure(EntityTypeBuilder<NoiTruHoSoKhacFileDinhKem> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruHoSoKhacFileDinhKemTable);

            builder.HasOne(rf => rf.NoiTruHoSoKhac)
                .WithMany(r => r.NoiTruHoSoKhacFileDinhKems)
                .HasForeignKey(rf => rf.NoiTruHoSoKhacId);

            base.Configure(builder);
        }
    }
}
