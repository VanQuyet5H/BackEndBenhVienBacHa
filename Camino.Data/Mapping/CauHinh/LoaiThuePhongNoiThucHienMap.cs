using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.CauHinhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CauHinh
{
    public class LoaiThuePhongNoiThucHienMap : CaminoEntityTypeConfiguration<LoaiThuePhongNoiThucHien>
    {
        public override void Configure(EntityTypeBuilder<LoaiThuePhongNoiThucHien> builder)
        {
            builder.ToTable(MappingDefaults.LoaiThuePhongNoiThucHienTable);      

            base.Configure(builder);
        }
    }
}
