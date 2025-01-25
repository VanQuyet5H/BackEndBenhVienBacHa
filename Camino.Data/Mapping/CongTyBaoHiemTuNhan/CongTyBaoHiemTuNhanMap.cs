using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CongTyBaoHiemTuNhan
{
    public class CongTyBaoHiemTuNhanMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.CongTyBaoHiemTuNhans.CongTyBaoHiemTuNhan>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.CongTyBaoHiemTuNhans.CongTyBaoHiemTuNhan> builder)
        {
            builder.ToTable(MappingDefaults.CongTyBaoHiemTuNhanTable);

            builder.HasMany(m => m.BenhNhanCongTyBaoHiemTuNhans)
                .WithOne(u => u.CongTyBaoHiemTuNhan)
                .HasForeignKey(m => m.CongTyBaoHiemTuNhanId)
                .IsRequired();

            builder.HasMany(m => m.YeuCauTiepNhanCongTyBaoHiemTuNhans)
              .WithOne(u => u.CongTyBaoHiemTuNhan)
              .HasForeignKey(m => m.CongTyBaoHiemTuNhanId)
              .IsRequired();

            base.Configure(builder);
        }
    }
}
