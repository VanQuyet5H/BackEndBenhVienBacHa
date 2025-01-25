using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.DichVuKhamBenh
{
    public class DichVuKhamBenhThongTinGiaMap : CaminoEntityTypeConfiguration<DichVuKhamBenhThongTinGia>
    {
        public override void Configure(EntityTypeBuilder<DichVuKhamBenhThongTinGia> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKhamBenhThongTinGiaTable);

            builder.HasOne(rf => rf.DichVuKhamBenh)
                .WithMany(r => r.DichVuKhamBenhThongTinGias)
                .HasForeignKey(rf => rf.DichVuKhamBenhId);

            base.Configure(builder);
        }
    }
}

