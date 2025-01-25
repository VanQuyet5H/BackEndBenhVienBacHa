using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class GoiKhamSucKhoeMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoe>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoe> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeTable);

            builder.HasOne(x => x.HopDongKhamSucKhoe)
                .WithMany(y => y.GoiKhamSucKhoes)
                .HasForeignKey(x => x.HopDongKhamSucKhoeId);
            base.Configure(builder);
        }
    }
}
