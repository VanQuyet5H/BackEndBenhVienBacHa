using Camino.Core.Domain.Entities.QuaTangs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.QuaTangs
{
    public class QuaTangMap : CaminoEntityTypeConfiguration<QuaTang>
    {
        public override void Configure(EntityTypeBuilder<QuaTang> builder)
        {
            builder.ToTable(MappingDefaults.QuaTangTable);

            base.Configure(builder);
        }
    }
}
