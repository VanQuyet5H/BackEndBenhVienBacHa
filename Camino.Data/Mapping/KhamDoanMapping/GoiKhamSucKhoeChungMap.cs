using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class GoiKhamSucKhoeChungMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeChung>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeChung> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeChungTable);

            base.Configure(builder);
        }
    }
}
