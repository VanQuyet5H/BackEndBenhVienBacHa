using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.CauHinh;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CauHinh
{
    public class CauHinhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.CauHinhs.CauHinh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.CauHinhs.CauHinh> builder)
        {
            builder.ToTable(MappingDefaults.CauHinhTable);
            base.Configure(builder);
        }
    }
}
