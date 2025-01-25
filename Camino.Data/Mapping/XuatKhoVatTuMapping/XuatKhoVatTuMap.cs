using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XuatKhoVatTuMapping
{
    public class XuatKhoVatTuMap : CaminoEntityTypeConfiguration<XuatKhoVatTu>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoVatTu> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoVatTuTable);

            builder.HasOne(m => m.KhoVatTuXuat)
              .WithMany(u => u.XuatKhoVatTuXuats)
              .HasForeignKey(m => m.KhoXuatId)
                .IsRequired();

            builder.HasOne(m => m.KhoVatTuNhap)
                .WithMany(u => u.XuatKhoVatTuNhaps)
                .HasForeignKey(m => m.KhoNhapId);

            builder.HasOne(m => m.KhoVatTuXuat)
              .WithMany(u => u.XuatKhoVatTuXuats)
              .HasForeignKey(m => m.KhoXuatId);

            builder.HasOne(m => m.KhoVatTuNhap)
                .WithMany(u => u.XuatKhoVatTuNhaps)
                .HasForeignKey(m => m.KhoNhapId);

            builder.HasOne(rf => rf.YeuCauLinhVatTu)
                .WithMany(r => r.XuatKhoVatTus)
                .HasForeignKey(rf => rf.YeuCauLinhVatTuId);

            builder.HasOne(rf => rf.NguoiNhan)
                .WithMany(r => r.XuatKhoVatTuNguoiNhans)
                .HasForeignKey(rf => rf.NguoiNhanId);

            builder.HasOne(rf => rf.NguoiXuat)
                .WithMany(r => r.XuatKhoVatTuNguoiXuats)
                .HasForeignKey(rf => rf.NguoiXuatId);

            builder.HasOne(m => m.NhaThau)
             .WithMany(u => u.XuatKhoVatTus)
             .HasForeignKey(m => m.NhaThauId);

            base.Configure(builder);
        }
    }
}
