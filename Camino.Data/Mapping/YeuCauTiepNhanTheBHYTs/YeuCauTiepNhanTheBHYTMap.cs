using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanTheBHYTs
{
    public class YeuCauTiepNhanTheBHYTMap : CaminoEntityTypeConfiguration<YeuCauTiepNhanTheBHYT>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTiepNhanTheBHYT> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTiepNhanTheBHYTTable);

            builder.HasOne(x => x.YeuCauTiepNhan)
                .WithMany(y => y.YeuCauTiepNhanTheBHYTs)
                .HasForeignKey(y => y.YeuCauTiepNhanId);

            builder.HasOne(x => x.BenhNhan)
                .WithMany(y => y.YeuCauTiepNhanTheBHYTs)
                .HasForeignKey(y => y.BenhNhanId);

            builder.HasOne(x => x.GiayMienCungChiTra)
                .WithMany(y => y.YeuCauTiepNhanTheBHYTs)
                .HasForeignKey(y => y.GiayMienCungChiTraId);


            base.Configure(builder);
        }
    }
}
