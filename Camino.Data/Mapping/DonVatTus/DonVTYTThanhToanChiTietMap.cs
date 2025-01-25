using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonVatTus
{
    public class DonVTYTThanhToanChiTietMap : CaminoEntityTypeConfiguration<DonVTYTThanhToanChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DonVTYTThanhToanChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DonVTYTThanhToanChiTietTable);

            builder.HasOne(rf => rf.DonVTYTThanhToan)
                .WithMany(r => r.DonVTYTThanhToanChiTiets)
                .HasForeignKey(rf => rf.DonVTYTThanhToanId);
            builder.HasOne(rf => rf.YeuCauKhamBenhDonVTYTChiTiet)
                .WithMany(r => r.DonVTYTThanhToanChiTiets)
                .HasForeignKey(rf => rf.YeuCauKhamBenhDonVTYTChiTietId);
            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.DonVTYTThanhToanChiTiets)
                .HasForeignKey(rf => rf.VatTuBenhVienId);
            builder.HasOne(rf => rf.XuatKhoVatTuChiTietViTri)
                .WithMany(r => r.DonVTYTThanhToanChiTiets)
                .HasForeignKey(rf => rf.XuatKhoVatTuChiTietViTriId);
            builder.HasOne(rf => rf.NhomVatTu)
                .WithMany(r => r.DonVTYTThanhToanChiTiets)
                .HasForeignKey(rf => rf.NhomVatTuId);
            builder.HasOne(rf => rf.HopDongThauVatTu)
                .WithMany(r => r.DonVTYTThanhToanChiTiets)
                .HasForeignKey(rf => rf.HopDongThauVatTuId);
            builder.HasOne(rf => rf.NhaThau)
                .WithMany(r => r.DonVTYTThanhToanChiTiets)
                .HasForeignKey(rf => rf.NhaThauId);

            base.Configure(builder);
        }
    }
}
