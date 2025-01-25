using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoVatTuMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHeSoVatTu>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHeSoVatTu> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHeSoVatTuTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoVatTus)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoVatTus)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            base.Configure(builder);
        }
    }
}
