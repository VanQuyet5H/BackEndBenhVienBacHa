using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class HopDongKhamSucKhoeMap : CaminoEntityTypeConfiguration<HopDongKhamSucKhoe>
    {
        public override void Configure(EntityTypeBuilder<HopDongKhamSucKhoe> builder)
        {
            builder.ToTable(MappingDefaults.HopDongKhamSucKhoeTable);

            builder.HasOne(x => x.CongTyKhamSucKhoe)
                .WithMany(y => y.HopDongKhamSucKhoes)
                .HasForeignKey(x => x.CongTyKhamSucKhoeId);


            builder.HasOne(x => x.NhanVienMoLaiHopDong)
                .WithMany(y => y.HopDongKhamSucKhoe)
                .HasForeignKey(x => x.NhanVienMoLaiHopDongId);


            base.Configure(builder);
        }
    }
}
