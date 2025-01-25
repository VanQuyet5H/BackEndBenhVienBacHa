using Camino.Core.Domain.Entities.Thuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.ToaThuocMaus
{
    public class ToaThuocMauMap : CaminoEntityTypeConfiguration<ToaThuocMau>
    {
        public override void Configure(EntityTypeBuilder<ToaThuocMau> builder)
        {
            builder.ToTable(MappingDefaults.ToaThuocMauTable);

            builder.HasOne(rf => rf.ICD)
              .WithMany(r => r.ToaThuocMaus)
              .HasForeignKey(rf => rf.ICDId);

            builder.HasOne(rf => rf.TrieuChung)
              .WithMany(r => r.ToaThuocMaus)
              .HasForeignKey(rf => rf.TrieuChungId);

            builder.HasOne(rf => rf.ChuanDoan)
              .WithMany(r => r.ToaThuocMaus)
              .HasForeignKey(rf => rf.ChuanDoanId);

            builder.HasOne(rf => rf.BacSiKeToa)
              .WithMany(r => r.ToaThuocMaus)
              .HasForeignKey(rf => rf.BacSiKeToaId);

            builder.HasMany(rf => rf.ToaThuocMauChiTiets)
              .WithOne(r => r.ToaThuocMau)
              .HasForeignKey(rf => rf.ToaThuocMauId)
              .IsRequired();

            base.Configure(builder);
        }
    }
}
