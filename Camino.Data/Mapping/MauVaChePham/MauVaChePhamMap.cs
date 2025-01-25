using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.MauVaChePham
{
    public class MauVaChePhamMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.MauVaChePhams.MauVaChePham>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.MauVaChePhams.MauVaChePham> builder)
        {
            builder.ToTable(MappingDefaults.MauVaChePhamTable);
            base.Configure(builder);
        }
    }
}
