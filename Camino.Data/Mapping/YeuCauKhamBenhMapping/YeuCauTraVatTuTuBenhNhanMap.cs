using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauTraVatTuTuBenhNhanMap : CaminoEntityTypeConfiguration<YeuCauTraVatTuTuBenhNhan>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraVatTuTuBenhNhan> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraVatTuTuBenhNhanTable);

            builder.HasOne(rf => rf.KhoTra)
                .WithMany(r => r.YeuCauTraVatTuTuBenhNhans)
                .HasForeignKey(rf => rf.KhoTraId);

            builder.HasOne(rf => rf.KhoaHoanTra)
                .WithMany(r => r.YeuCauTraVatTuTuBenhNhans)
                .HasForeignKey(rf => rf.KhoaHoanTraId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.NhanVienYeuCauYeuCauTraVatTuTuBenhNhans)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.NhanVienDuyetYeuCauTraVatTuTuBenhNhans)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
