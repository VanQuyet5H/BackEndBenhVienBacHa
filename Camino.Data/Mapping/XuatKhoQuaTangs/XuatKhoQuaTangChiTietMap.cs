using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XuatKhoQuaTangs
{
    public class XuatKhoQuaTangChiTietMap : CaminoEntityTypeConfiguration<XuatKhoQuaTangChiTiet>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoQuaTangChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoQuaTangChiTietTable);

            builder
                .HasOne(sc => sc.XuatKhoQuaTang)
                .WithMany(s => s.XuatKhoQuaTangChiTiet)
                .HasForeignKey(sc => sc.XuatKhoQuaTangId);

            builder
                .HasOne(sc => sc.QuaTang)
                .WithMany(s => s.XuatKhoQuaTangChiTiet)
                .HasForeignKey(sc => sc.QuaTangId);

            builder
                .HasOne(sc => sc.NhapKhoQuaTangChiTiet)
                .WithMany(s => s.XuatKhoQuaTangChiTiet)
                .HasForeignKey(sc => sc.NhapKhoQuaTangChiTietId);

            base.Configure(builder);
        }
    }
}
