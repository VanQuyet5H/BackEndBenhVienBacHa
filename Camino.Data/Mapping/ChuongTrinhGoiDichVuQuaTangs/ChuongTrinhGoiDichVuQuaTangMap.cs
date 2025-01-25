using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuQuaTangs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.ChuongTrinhGoiDichVuQuaTangs
{
    public class ChuongTrinhGoiDichVuQuaTangMap : CaminoEntityTypeConfiguration<ChuongTrinhGoiDichVuQuaTang>
    {
        public override void Configure(EntityTypeBuilder<ChuongTrinhGoiDichVuQuaTang> builder)
        {
            builder.ToTable(MappingDefaults.ChuongTrinhGoiDichVuQuaTangTable);

            builder
                .HasOne(sc => sc.ChuongTrinhGoiDichVu)
                .WithMany(s => s.ChuongTrinhGoiDichVuQuaTangs)
                .HasForeignKey(sc => sc.ChuongTrinhGoiDichVuId);

            builder
                .HasOne(sc => sc.QuaTang)
                .WithMany(s => s.ChuongTrinhGoiDichVuQuaTangs)
                .HasForeignKey(sc => sc.QuaTangId);


            base.Configure(builder);
        }
    }
}
