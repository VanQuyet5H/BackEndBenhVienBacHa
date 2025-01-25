using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KetQuaVaKetLuanMaus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KetQuaVaKetLuanMauMapping
{
    public class KetQuaVaKetLuanMauMap : CaminoEntityTypeConfiguration<KetQuaVaKetLuanMau>
    {
        public override void Configure(EntityTypeBuilder<KetQuaVaKetLuanMau> builder)
        {
            builder.ToTable(MappingDefaults.KetQuaVaKetLuanMauTable);

            base.Configure(builder);
        }
    }
}
