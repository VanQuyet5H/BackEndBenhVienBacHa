using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhoVatTuMapping
{
    public class XuatKhoVatTuChiTietViTriMap : CaminoEntityTypeConfiguration<XuatKhoVatTuChiTietViTri>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoVatTuChiTietViTri> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoVatTuChiTietViTriTable);

            builder.HasOne(m => m.XuatKhoVatTuChiTiet)
                .WithMany(u => u.XuatKhoVatTuChiTietViTris)
                .HasForeignKey(m => m.XuatKhoVatTuChiTietId);
            builder.HasOne(m => m.NhapKhoVatTuChiTiet)
                .WithMany(u => u.XuatKhoVatTuChiTietViTris)
                .HasForeignKey(m => m.NhapKhoVatTuChiTietId);

            builder.HasOne(m => m.YeuCauTraVatTuTuBenhNhanChiTiet)
                .WithMany(u => u.XuatKhoVatTuChiTietViTris)
                .HasForeignKey(m => m.YeuCauTraVatTuTuBenhNhanChiTietId);

            base.Configure(builder);
        }
    }
}
