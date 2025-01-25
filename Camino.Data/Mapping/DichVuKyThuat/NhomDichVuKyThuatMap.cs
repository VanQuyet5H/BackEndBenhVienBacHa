using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class NhomDichVuKyThuatMap: CaminoEntityTypeConfiguration<NhomDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<NhomDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.NhomDichVuKyThuatTable);

            base.Configure(builder);
        }
    }
}
