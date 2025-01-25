using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruEkipDieuTriMap : CaminoEntityTypeConfiguration<NoiTruEkipDieuTri>
    {
        public override void Configure(EntityTypeBuilder<NoiTruEkipDieuTri> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruEkipDieuTriTable);

            builder.HasOne(rf => rf.NoiTruBenhAn)
                .WithMany(r => r.NoiTruEkipDieuTris)
                .HasForeignKey(rf => rf.NoiTruBenhAnId);

            builder.HasOne(rf => rf.BacSi)
                .WithMany(r => r.BacSiNoiTruEkipDieuTris)
                .HasForeignKey(rf => rf.BacSiId);

            builder.HasOne(rf => rf.DieuDuong)
                .WithMany(r => r.DieuDuongNoiTruEkipDieuTris)
                .HasForeignKey(rf => rf.DieuDuongId);

            builder.HasOne(rf => rf.NhanVienLap)
                .WithMany(r => r.NhanVienLapNoiTruEkipDieuTris)
                .HasForeignKey(rf => rf.NhanVienLapId);

            builder.HasOne(rf => rf.KhoaPhongDieuTri)
                .WithMany(r => r.NoiTruEkipDieuTris)
                .HasForeignKey(rf => rf.KhoaPhongDieuTriId);

            base.Configure(builder);
        }
    }
}
