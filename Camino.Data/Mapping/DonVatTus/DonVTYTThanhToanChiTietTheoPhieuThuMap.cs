using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonVatTus
{
    public class DonVTYTThanhToanChiTietTheoPhieuThuMap : CaminoEntityTypeConfiguration<DonVTYTThanhToanChiTietTheoPhieuThu>
    {
        public override void Configure(EntityTypeBuilder<DonVTYTThanhToanChiTietTheoPhieuThu> builder)
        {
            builder.ToTable(MappingDefaults.DonVTYTThanhToanChiTietTheoPhieuThuTable);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.DonVTYTThanhToanChiTietTheoPhieuThus)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.NhomVatTu)
                .WithMany()
                .HasForeignKey(rf => rf.NhomVatTuId);

            builder.HasOne(rf => rf.BacSiKeDon)
                .WithMany()
                .HasForeignKey(rf => rf.BacSiKeDonId);


            base.Configure(builder);
        }
    }
}
