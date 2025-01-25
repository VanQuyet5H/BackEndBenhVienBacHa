using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonVatTus
{
    public class DonVTYTThanhToanMap : CaminoEntityTypeConfiguration<DonVTYTThanhToan>
    {
        public override void Configure(EntityTypeBuilder<DonVTYTThanhToan> builder)
        {
            builder.ToTable(MappingDefaults.DonVTYTThanhToanTable);

            builder.HasOne(rf => rf.YeuCauKhamBenhDonVTYT)
                .WithMany(r => r.DonVTYTThanhToans)
                .HasForeignKey(rf => rf.YeuCauKhamBenhDonVTYTId);
            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.DonVTYTThanhToans)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.DonVTYTThanhToans)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.BenhNhan)
                .WithMany(r => r.DonVTYTThanhToans)
                .HasForeignKey(rf => rf.BenhNhanId);
            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.DonVTYTThanhToanBiHuys)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);
            builder.HasOne(rf => rf.NoiCapVTYT)
                .WithMany(r => r.DonVTYTThanhToans)
                .HasForeignKey(rf => rf.NoiCapVTYTId);
            builder.HasOne(rf => rf.NhanVienCapVTYT)
                .WithMany(r => r.DonVTYTThanhToans)
                .HasForeignKey(rf => rf.NhanVienCapVTYTId);

            base.Configure(builder);
        }
    }
}
