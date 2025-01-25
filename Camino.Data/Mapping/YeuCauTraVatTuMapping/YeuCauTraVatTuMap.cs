using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauTraVatTuMapping
{
    public class YeuCauTraVatTuMap : CaminoEntityTypeConfiguration<YeuCauTraVatTu>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraVatTu> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraVatTuTable);

            builder.HasOne(rf => rf.KhoXuat)
                .WithMany(r => r.YeuCauTraVatTuKhoXuats)
                .HasForeignKey(rf => rf.KhoXuatId);

            builder.HasOne(rf => rf.KhoNhap)
                .WithMany(r => r.YeuCauTraVatTuKhoNhaps)
                .HasForeignKey(rf => rf.KhoNhapId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.YeuCauTraVatTuNhanVienYeuCaus)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.YeuCauTraVatTuNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
