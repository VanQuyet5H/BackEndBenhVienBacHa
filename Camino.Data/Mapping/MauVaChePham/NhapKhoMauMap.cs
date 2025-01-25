using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MauVaChePham
{
    public class NhapKhoMauMap : CaminoEntityTypeConfiguration<NhapKhoMau>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoMau> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoMauTable);

            builder.HasOne(rf => rf.NguoiGiao)
                .WithMany(r => r.NguoiGiaoNhapKhoMaus)
                .HasForeignKey(rf => rf.NguoiGiaoId);
            builder.HasOne(rf => rf.NhaThau)
                .WithMany(r => r.NhapKhoMaus)
                .HasForeignKey(rf => rf.NhaThauId);
            builder.HasOne(rf => rf.NguoiNhap)
                .WithMany(r => r.NguoiNhapNhapKhoMaus)
                .HasForeignKey(rf => rf.NguoiNhapId);
            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.NhanVienDuyetNhapKhoMaus)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
