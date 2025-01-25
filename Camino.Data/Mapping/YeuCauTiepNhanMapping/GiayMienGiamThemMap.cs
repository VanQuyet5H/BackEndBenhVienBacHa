using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class GiayMienGiamThemMap : CaminoEntityTypeConfiguration<GiayMienGiamThem>
    {
        public override void Configure(EntityTypeBuilder<GiayMienGiamThem> builder)
        {
            builder.ToTable(MappingDefaults.GiayMienGiamThemTable);
            base.Configure(builder);
        }
    }
}
