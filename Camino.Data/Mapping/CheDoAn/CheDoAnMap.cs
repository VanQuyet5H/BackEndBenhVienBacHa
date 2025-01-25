using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.CheDoAn
{
    public class CheDoAnMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.CheDoAns.CheDoAn>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.CheDoAns.CheDoAn> builder)
        {
            builder.ToTable(MappingDefaults.CheDoAnTable);
            base.Configure(builder);
        }
    }
}
