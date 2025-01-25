using Camino.Core.Domain.Entities.VatTuBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.VatTuBenhViens
{
    public class VatTuBenhVienMap : CaminoEntityTypeConfiguration<VatTuBenhVien>
    {
        public override void Configure(EntityTypeBuilder<VatTuBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.VatTuBenhVienTable);

            builder.HasOne(rf => rf.VatTus)
                .WithOne(r => r.VatTuBenhVien).
                HasForeignKey<VatTuBenhVien>(c => c.Id);

            builder.HasMany(rf => rf.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
               .WithOne(r => r.VatTuBenhVien)
               .HasForeignKey(rf => rf.VatTuBenhVienId);

            base.Configure(builder);
        }
    }
}
