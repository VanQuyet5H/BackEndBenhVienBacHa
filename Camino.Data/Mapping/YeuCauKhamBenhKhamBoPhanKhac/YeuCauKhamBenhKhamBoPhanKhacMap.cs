using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauKhamBenhKhamBoPhanKhac
{
    public class YeuCauKhamBenhKhamBoPhanKhacMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhKhamBoPhanKhacTable);
            builder.HasOne(rf => rf.YeuCauKhamBenh)
               .WithMany(r => r.YeuCauKhamBenhKhamBoPhanKhacs)
               .HasForeignKey(rf => rf.YeuCauKhamBenhId);
            base.Configure(builder);
        }
    }
}
