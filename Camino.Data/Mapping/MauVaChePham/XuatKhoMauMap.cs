using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MauVaChePham
{
    public class XuatKhoMauMap : CaminoEntityTypeConfiguration<XuatKhoMau>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoMau> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoMauTable);

            builder.HasOne(rf => rf.NguoiXuat)
                .WithMany(r => r.NguoiXuatXuatKhoMaus)
                .HasForeignKey(rf => rf.NguoiXuatId);
            builder.HasOne(rf => rf.NguoiNhan)
                .WithMany(r => r.NguoiNhanXuatKhoMaus)
                .HasForeignKey(rf => rf.NguoiNhanId);

            base.Configure(builder);
        }
    }
}
