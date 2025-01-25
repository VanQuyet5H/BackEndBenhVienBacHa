using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruPhieuDieuTriMap : CaminoEntityTypeConfiguration<NoiTruPhieuDieuTri>
    {
        public override void Configure(EntityTypeBuilder<NoiTruPhieuDieuTri> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruPhieuDieuTriTable);

            builder.HasOne(rf => rf.NoiTruBenhAn)
                .WithMany(r => r.NoiTruPhieuDieuTris)
                .HasForeignKey(rf => rf.NoiTruBenhAnId);

            builder.HasOne(rf => rf.NhanVienLap)
                .WithMany(r => r.NoiTruPhieuDieuTris)
                .HasForeignKey(rf => rf.NhanVienLapId);

            builder.HasOne(rf => rf.KhoaPhongDieuTri)
                .WithMany(r => r.NoiTruPhieuDieuTris)
                .HasForeignKey(rf => rf.KhoaPhongDieuTriId);

            builder.HasOne(rf => rf.ChanDoanChinhICD)
                .WithMany(r => r.NoiTruPhieuDieuTris)
                .HasForeignKey(rf => rf.ChanDoanChinhICDId);

            builder.HasOne(rf => rf.CheDoAn)
                .WithMany(r => r.NoiTruPhieuDieuTris)
                .HasForeignKey(rf => rf.CheDoAnId);

            base.Configure(builder);
        }
    }
}
