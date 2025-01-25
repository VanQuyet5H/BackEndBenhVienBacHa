using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhaSanXuat
{
    public class NhaSanXuatMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.NhaSanXuats.NhaSanXuat>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.NhaSanXuats.NhaSanXuat> builder)
        {
            builder.ToTable(MappingDefaults.NhaSanXuatTable);
            base.Configure(builder);
        }
    }
}
