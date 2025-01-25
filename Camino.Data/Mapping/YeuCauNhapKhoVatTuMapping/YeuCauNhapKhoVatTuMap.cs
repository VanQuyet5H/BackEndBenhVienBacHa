using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauNhapKhoVatTuMapping
{
    public class YeuCauNhapKhoVatTuMap : CaminoEntityTypeConfiguration<YeuCauNhapKhoVatTu>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhapKhoVatTu> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhapKhoVatTuTable);

            builder.HasOne(rf => rf.Kho)
                .WithMany(r => r.YeuCauNhapKhoVatTus)
                .HasForeignKey(rf => rf.KhoId);

            builder.HasOne(rf => rf.NguoiGiao)
                .WithMany(r => r.YeuCauNhapKhoVatTuNguoiGiaos)
                .HasForeignKey(rf => rf.NguoiGiaoId);

            builder.HasOne(rf => rf.NguoiNhap)
                .WithMany(r => r.YeuCauNhapKhoVatTuNguoiNhaps)
                .HasForeignKey(rf => rf.NguoiNhapId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.YeuCauNhapKhoVatTuNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
