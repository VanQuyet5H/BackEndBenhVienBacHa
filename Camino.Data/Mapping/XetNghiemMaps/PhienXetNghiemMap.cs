using Camino.Core.Domain.Entities.XetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XetNghiemMaps
{
    public class PhienXetNghiemMap : CaminoEntityTypeConfiguration<PhienXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<PhienXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.PhienXetNghiemTable);

            builder.HasOne(rf => rf.BenhNhan)
                .WithMany(r => r.PhienXetNghiems)
                .HasForeignKey(rf => rf.BenhNhanId);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.PhienXetNghiems)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.PhongThucHien)
                .WithMany(r => r.PhienXetNghiems)
                .HasForeignKey(rf => rf.PhongThucHienId);

            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.PhienXetNghiemNhanVienThucHiens)
                .HasForeignKey(rf => rf.NhanVienThucHienId);

            builder.HasOne(rf => rf.NhanVienKetLuan)
                .WithMany(r => r.PhienXetNghiemNhanVienKetLuans)
                .HasForeignKey(rf => rf.NhanVienKetLuanId);

            base.Configure(builder);
        }
    }
}
