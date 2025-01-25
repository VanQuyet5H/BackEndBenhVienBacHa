using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Thuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Thuocs
{
    public class DuongDungMap: CaminoEntityTypeConfiguration<DuongDung>
    {
        public override void Configure(EntityTypeBuilder<DuongDung> builder)
        {
            builder.ToTable(MappingDefaults.DuongDungTable);


            base.Configure(builder);
        }
    }
}
