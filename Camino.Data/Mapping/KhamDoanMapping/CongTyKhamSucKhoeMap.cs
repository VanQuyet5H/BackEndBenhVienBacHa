using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class CongTyKhamSucKhoeMap : CaminoEntityTypeConfiguration<CongTyKhamSucKhoe>
    {
        public override void Configure(EntityTypeBuilder<CongTyKhamSucKhoe> builder)
        {
            builder.ToTable(MappingDefaults.CongTyKhamSucKhoeTable);

            base.Configure(builder);
        }
    }
}
