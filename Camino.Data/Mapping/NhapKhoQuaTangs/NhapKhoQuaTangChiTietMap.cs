using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhapKhoQuaTangs
{
    public class NhapKhoQuaTangChiTietMap : CaminoEntityTypeConfiguration<NhapKhoQuaTangChiTiet>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoQuaTangChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoQuaTangChiTietTable);

            builder
                .HasOne(sc => sc.NhapKhoQuaTang)
                .WithMany(s => s.NhapKhoQuaTangChiTiets)
                .HasForeignKey(sc => sc.NhapKhoQuaTangId);

            builder
                .HasOne(sc => sc.QuaTang)
                .WithMany(s => s.NhapKhoQuaTangChiTiets)
                .HasForeignKey(sc => sc.QuaTangId);

            base.Configure(builder);
        }
    }
}
