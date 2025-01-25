using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.HeThong;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.QuanLyNhatKyHeThong
{
    public class QuanLyNhatKyHeThongMap:CaminoEntityTypeConfiguration<NhatKyHeThong>
    {
        public override void Configure(EntityTypeBuilder<NhatKyHeThong> builder)
        {
            builder.ToTable(MappingDefaults.NhatKyHeThongTable);

            builder.HasOne(rf => rf.UserDetails)
                .WithMany(r => r.NhatKyHeThongs)
                .HasForeignKey(rf => rf.CreatedById)
                .IsRequired();

            builder.Property(x => x.NoiDung).HasMaxLength(2000);
            builder.Property(x => x.MaDoiTuong).HasMaxLength(20);
            base.Configure(builder);
        }
    }
}
