using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class ICDDoiTuongBenhNhanMap : CaminoEntityTypeConfiguration<ICDDoiTuongBenhNhan>
    {
        public override void Configure(EntityTypeBuilder<ICDDoiTuongBenhNhan> builder)
        {
            builder.ToTable(MappingDefaults.ICDDoiTuongBenhNhanTable);

            builder.Property(x => x.Ten).HasMaxLength(250);
            builder.Property(x => x.GhiChu).HasMaxLength(250);

            base.Configure(builder);
        }
    }
}
