using Camino.Core.Domain.Entities.CauHinhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.CauHinh
{
    public class LoaiThuePhongPhauThuatMap : CaminoEntityTypeConfiguration<LoaiThuePhongPhauThuat>
    {
        public override void Configure(EntityTypeBuilder<LoaiThuePhongPhauThuat> builder)
        {
            builder.ToTable(MappingDefaults.LoaiThuePhongPhauThuatTable);     

            base.Configure(builder);
        }
}
}
