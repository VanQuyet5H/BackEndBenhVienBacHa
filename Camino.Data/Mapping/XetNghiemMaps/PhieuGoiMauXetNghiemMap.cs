using Camino.Core.Domain.Entities.XetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XetNghiemMaps
{
    public class PhieuGoiMauXetNghiemMap : CaminoEntityTypeConfiguration<PhieuGoiMauXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<PhieuGoiMauXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.PhieuGoiMauXetNghiemTable);

            builder.HasOne(rf => rf.NhanVienGoiMau)
                .WithMany(r => r.PhieuGoiMauXetNghiemNhanVienGoiMaus)
                .HasForeignKey(rf => rf.NhanVienGoiMauId);

            builder.HasOne(rf => rf.NhanVienNhanMau)
                .WithMany(r => r.PhieuGoiMauXetNghiemNhanVienNhanMaus)
                .HasForeignKey(rf => rf.NhanVienNhanMauId);

            builder.HasOne(rf => rf.PhongNhanMau)
                .WithMany(r => r.PhieuGoiMauXetNghiems)
                .HasForeignKey(rf => rf.PhongNhanMauId);

            base.Configure(builder);
        }
    }
}
