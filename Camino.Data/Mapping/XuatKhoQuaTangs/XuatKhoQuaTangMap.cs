using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XuatKhoQuaTangs
{
    public class XuatKhoQuaTangMap : CaminoEntityTypeConfiguration<XuatKhoQuaTang>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoQuaTang> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoQuaTangTable);

            builder
                .HasOne(sc => sc.NguoiXuat)
                .WithMany(s => s.XuatKhoQuaTangs)
                .HasForeignKey(sc => sc.NguoiXuatId);

            builder
                .HasOne(sc => sc.BenhNhan)
                .WithMany(s => s.XuatKhoQuaTangs)
                .HasForeignKey(sc => sc.BenhNhanId);

            builder
                .HasOne(sc => sc.YeuCauGoiDichVu)
                .WithMany(s => s.XuatKhoQuaTangs)
                .HasForeignKey(sc => sc.YeuCauGoiDichVuId);

            base.Configure(builder);
        }
    }
}
