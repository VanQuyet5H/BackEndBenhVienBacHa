using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CauHinh
{
    public class CauHinhThapGiaMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.CauHinhs.CauHinhThapGia>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.CauHinhs.CauHinhThapGia> builder)
        {
            builder.ToTable(MappingDefaults.CauHinhThapGiaTable);
            base.Configure(builder);
        }
    }
}
