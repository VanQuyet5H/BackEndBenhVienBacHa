using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MauVaChePham
{
    public class XuatKhoMauChiTietMap : CaminoEntityTypeConfiguration<XuatKhoMauChiTiet>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoMauChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoMauChiTietTable);
            builder.HasOne(rf => rf.XuatKhoMau)
                .WithMany(r => r.XuatKhoMauChiTiets)
                .HasForeignKey(rf => rf.XuatKhoMauId);
            builder.HasOne(rf => rf.NhapKhoMauChiTiet)
                .WithMany(r => r.XuatKhoMauChiTiets)
                .HasForeignKey(rf => rf.NhapKhoMauChiTietId);
            base.Configure(builder);
        }
    }
}
