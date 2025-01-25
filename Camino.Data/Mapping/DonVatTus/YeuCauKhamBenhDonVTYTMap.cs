using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonVatTus
{
    public class YeuCauKhamBenhDonVTYTMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhDonVTYT>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhDonVTYT> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhDonVTYTTable);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.YeuCauKhamBenhDonVTYTs)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.NoiKeDon)
                .WithMany(r => r.YeuCauKhamBenhDonVTYTNoiKeDons)
                .HasForeignKey(rf => rf.NoiKeDonId);

            builder.HasOne(rf => rf.BacSiKeDon)
                .WithMany(r => r.YeuCauKhamBenhDonVTYTBacSiKeDons)
                .HasForeignKey(rf => rf.BacSiKeDonId);

            base.Configure(builder);
        }
    }
}
