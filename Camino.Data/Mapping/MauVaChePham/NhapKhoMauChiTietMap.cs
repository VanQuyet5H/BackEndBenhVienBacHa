using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MauVaChePham
{
    public class NhapKhoMauChiTietMap : CaminoEntityTypeConfiguration<NhapKhoMauChiTiet>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoMauChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoMauChiTietTable);

            builder.HasOne(m => m.NhapKhoMau)
                .WithMany(u => u.NhapKhoMauChiTiets)
                .HasForeignKey(m => m.NhapKhoMauId);
            builder.HasOne(m => m.YeuCauTruyenMau)
                .WithMany(u => u.NhapKhoMauChiTiets)
                .HasForeignKey(m => m.YeuCauTruyenMauId);
            builder.HasOne(m => m.MauVaChePham)
                .WithMany(u => u.NhapKhoMauChiTiets)
                .HasForeignKey(m => m.MauVaChePhamId);           

            base.Configure(builder);
        }
    }
}
