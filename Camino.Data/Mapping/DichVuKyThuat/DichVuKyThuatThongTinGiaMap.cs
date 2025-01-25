using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatThongTinGiaMap : CaminoEntityTypeConfiguration<DichVuKyThuatThongTinGia>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatThongTinGia> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatThongTinGiaTable);

            builder.HasOne(rf => rf.DichVuKyThuat)
                .WithMany(r => r.DichVuKyThuatThongTinGias)
                .HasForeignKey(rf => rf.DichVuKyThuatId);

            base.Configure(builder);
        }
    }
}
