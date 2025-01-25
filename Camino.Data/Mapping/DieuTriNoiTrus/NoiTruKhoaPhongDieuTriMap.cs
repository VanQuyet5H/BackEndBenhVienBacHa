using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruKhoaPhongDieuTriMap : CaminoEntityTypeConfiguration<NoiTruKhoaPhongDieuTri>
    {
        public override void Configure(EntityTypeBuilder<NoiTruKhoaPhongDieuTri> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruKhoaPhongDieuTriTable);

            builder.HasOne(rf => rf.NoiTruBenhAn)
                .WithMany(r => r.NoiTruKhoaPhongDieuTris)
                .HasForeignKey(rf => rf.NoiTruBenhAnId);

            builder.HasOne(rf => rf.KhoaPhongChuyenDi)
                .WithMany(r => r.KhoaPhongChuyenDiNoiTruKhoaPhongDieuTris)
                .HasForeignKey(rf => rf.KhoaPhongChuyenDiId);

            builder.HasOne(rf => rf.KhoaPhongChuyenDen)
                .WithMany(r => r.KhoaPhongChuyenDenNoiTruKhoaPhongDieuTris)
                .HasForeignKey(rf => rf.KhoaPhongChuyenDenId);

            builder.HasOne(rf => rf.ChanDoanVaoKhoaICD)
                .WithMany(r => r.NoiTruKhoaPhongDieuTris)
                .HasForeignKey(rf => rf.ChanDoanVaoKhoaICDId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                .WithMany(r => r.NoiTruKhoaPhongDieuTris)
                .HasForeignKey(rf => rf.NhanVienChiDinhId);

            base.Configure(builder);
        }
    }
}
