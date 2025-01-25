using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class TemplateDichVuKhamSucKhoeMap : CaminoEntityTypeConfiguration<TemplateDichVuKhamSucKhoe>
    {
        public override void Configure(EntityTypeBuilder<TemplateDichVuKhamSucKhoe> builder)
        {
            builder.ToTable(MappingDefaults.TemplateDichVuKhamSucKhoeTable);

            base.Configure(builder);
        }
    }
}
