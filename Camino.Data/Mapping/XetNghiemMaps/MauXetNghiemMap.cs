using Camino.Core.Domain.Entities.XetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XetNghiemMaps
{
    public class MauXetNghiemMap : CaminoEntityTypeConfiguration<MauXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<MauXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.MauXetNghiemTable);

            builder.HasOne(rf => rf.PhienXetNghiem)
                .WithMany(r => r.MauXetNghiems)
                .HasForeignKey(rf => rf.PhienXetNghiemId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.MauXetNghiems)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            builder.HasOne(rf => rf.PhongLayMau)
                .WithMany(r => r.MauXetNghiems)
                .HasForeignKey(rf => rf.PhongLayMauId);

            builder.HasOne(rf => rf.NhanVienLayMau)
                .WithMany(r => r.MauXetNghiemNhanVienLayMaus)
                .HasForeignKey(rf => rf.NhanVienLayMauId);

            builder.HasOne(rf => rf.NhanVienXetKhongDat)
                .WithMany(r => r.MauXetNghiemNhanVienXetKhongDats)
                .HasForeignKey(rf => rf.NhanVienXetKhongDatId);

            builder.HasOne(rf => rf.PhieuGoiMauXetNghiem)
               .WithMany(r => r.MauXetNghiems)
               .HasForeignKey(rf => rf.PhieuGoiMauXetNghiemId);

            builder.HasOne(rf => rf.PhongNhanMau)
                .WithMany(r => r.MauXetNghiemPhongNhanMaus)
                .HasForeignKey(rf => rf.PhongNhanMauId);

            builder.HasOne(rf => rf.NhanVienNhanMau)
                .WithMany(r => r.MauXetNghiemNhanVienNhanMaus)
                .HasForeignKey(rf => rf.NhanVienNhanMauId);

            base.Configure(builder);
        }
    }
}
