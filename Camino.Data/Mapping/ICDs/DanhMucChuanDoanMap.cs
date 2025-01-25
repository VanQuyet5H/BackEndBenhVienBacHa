using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class DanhMucChuanDoanMap : CaminoEntityTypeConfiguration<DanhMucChuanDoan>
    {
        public override void Configure(EntityTypeBuilder<DanhMucChuanDoan> builder)
        {
            builder.ToTable(MappingDefaults.DanhMucChuanDoanTable);

            base.Configure(builder);
        }
    }
}
