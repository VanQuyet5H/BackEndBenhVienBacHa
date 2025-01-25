using Camino.Core.Domain.Entities.CauHinhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.CauHinh
{
    public class NgayLeTetMap : CaminoEntityTypeConfiguration<NgayLeTet>
    {
        public override void Configure(EntityTypeBuilder<NgayLeTet> builder)
        {
            builder.ToTable(MappingDefaults.NgayLeTetTable);
       
            base.Configure(builder);
        }
    }
}
