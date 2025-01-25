using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruThoiGianDieuTriBenhAnSoSinhMap : CaminoEntityTypeConfiguration<NoiTruThoiGianDieuTriBenhAnSoSinh>
    {
        public override void Configure(EntityTypeBuilder<NoiTruThoiGianDieuTriBenhAnSoSinh> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruThoiGianDieuTriBenhAnSoSinhTable);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
                .WithMany(r => r.NoiTruThoiGianDieuTriBenhAnSoSinhs)
                .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.NoiTruBenhAn)
                .WithMany(r => r.NoiTruThoiGianDieuTriBenhAnSoSinhs)
                .HasForeignKey(rf => rf.NoiTruBenhAnId);

            base.Configure(builder);
        }
    }
}
