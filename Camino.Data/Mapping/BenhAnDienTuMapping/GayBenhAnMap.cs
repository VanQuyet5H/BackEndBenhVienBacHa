using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhAnDienTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhAnDienTuMapping
{
    public class GayBenhAnMap: CaminoEntityTypeConfiguration<GayBenhAn>
    {
        public override void Configure(EntityTypeBuilder<GayBenhAn> builder)
        {
            builder.ToTable(MappingDefaults.GayBenhAnTable);

            base.Configure(builder);
        }
    }
}
