using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.DichVuGiuong
{
  
    public class DichVuGiuongMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DichVuGiuongs.DichVuGiuong> builder)
        {
            builder.ToTable(MappingDefaults.DichVuGiuongTable);

            builder.HasMany(rf => rf.DichVuGiuongThongTinGias)
                .WithOne(r => r.DichVuGiuong)
                .HasForeignKey(rf => rf.DichVuGiuongId);

            builder
                .HasOne(sc => sc.Khoa)
                .WithMany(s => s.DichVuGiuongs)
                .HasForeignKey(sc => sc.KhoaId);

            base.Configure(builder);
        }
    }
}
