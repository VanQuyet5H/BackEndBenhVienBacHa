using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class HopDongKhamSucKhoeDiaDiemMap : CaminoEntityTypeConfiguration<HopDongKhamSucKhoeDiaDiem>
    {
        public override void Configure(EntityTypeBuilder<HopDongKhamSucKhoeDiaDiem> builder)
        {
            builder.ToTable(MappingDefaults.HopDongKhamSucKhoeDiaDiemTable);

            builder.HasOne(x => x.HopDongKhamSucKhoe)
                .WithMany(y => y.HopDongKhamSucKhoeDiaDiems)
                .HasForeignKey(x => x.HopDongKhamSucKhoeId);

            base.Configure(builder);
        }
    }
}
