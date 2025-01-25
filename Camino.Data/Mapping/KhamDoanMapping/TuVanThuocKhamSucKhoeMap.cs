using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class TuVanThuocKhamSucKhoeMap : CaminoEntityTypeConfiguration<TuVanThuocKhamSucKhoe>
    {
        public override void Configure(EntityTypeBuilder<TuVanThuocKhamSucKhoe> builder)
        {
            builder.ToTable(MappingDefaults.TuVanThuocKhamSucKhoeTable);

            builder.HasOne(x => x.YeuCauTiepNhan)
                .WithMany(y => y.TuVanThuocKhamSucKhoes)
                .HasForeignKey(x => x.YeuCauTiepNhanId);

            builder.HasOne(x => x.DuocPham)
                .WithMany(y => y.TuVanThuocKhamSucKhoes)
                .HasForeignKey(x => x.DuocPhamId);

            builder.HasOne(x => x.DuongDung)
                .WithMany(y => y.TuVanThuocKhamSucKhoes)
                .HasForeignKey(x => x.DuongDungId);

            builder.HasOne(x => x.DonViTinh)
                .WithMany(y => y.TuVanThuocKhamSucKhoes)
                .HasForeignKey(x => x.DonViTinhId);
            base.Configure(builder);
        }
    }
}
