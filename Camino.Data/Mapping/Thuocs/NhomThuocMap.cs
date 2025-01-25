using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Thuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Thuocs
{
    public class NhomThuocMap : CaminoEntityTypeConfiguration<NhomThuoc>
    {
        public override void Configure(EntityTypeBuilder<NhomThuoc> builder)
        {
            builder.ToTable(MappingDefaults.NhomThuocTable);

            //builder.HasOne(rf => rf.NhomCha)
            //  .WithOne(r => r.NhomChas)
            //  .HasForeignKey<Core.Domain.Entities.Thuocs.NhomThuoc>(rf => rf.NhomChaId);

            base.Configure(builder);
        }
    }
}
