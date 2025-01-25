using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.GachNos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GachNoMapping
{
    public class GachNoMap : CaminoEntityTypeConfiguration<GachNo>
    {
        public override void Configure(EntityTypeBuilder<GachNo> builder)
        {
            builder.ToTable(MappingDefaults.GachNoTable);

            builder.HasOne(x => x.BenhNhan)
                .WithMany(y => y.GachNos)
                .HasForeignKey(x => x.BenhNhanId);
            builder.HasOne(x => x.CongTyBaoHiemTuNhan)
                .WithMany(y => y.GachNos)
                .HasForeignKey(x => x.CongTyBaoHiemTuNhanId);

            builder.HasOne(x => x.NguoiXacNhanNhapLieu)
                .WithMany(y => y.GachNos)
                .HasForeignKey(x => x.CreatedById);

            base.Configure(builder);
        }
    }
}
