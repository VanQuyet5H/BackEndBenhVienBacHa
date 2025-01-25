using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauTraDuocPhamTuBenhNhanMap : CaminoEntityTypeConfiguration<YeuCauTraDuocPhamTuBenhNhan>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTraDuocPhamTuBenhNhan> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTraDuocPhamTuBenhNhanTable);

            builder.HasOne(rf => rf.KhoTra)
                .WithMany(r => r.YeuCauTraDuocPhamTuBenhNhans)
                .HasForeignKey(rf => rf.KhoTraId);

            builder.HasOne(rf => rf.KhoaHoanTra)
                .WithMany(r => r.YeuCauTraDuocPhamTuBenhNhans)
                .HasForeignKey(rf => rf.KhoaHoanTraId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.NhanVienYeuCauYeuCauTraDuocPhamTuBenhNhans)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.NhanVienDuyetYeuCauTraDuocPhamTuBenhNhans)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
