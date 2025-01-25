using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class HoSoYeuCauTiepNhanMap : CaminoEntityTypeConfiguration<HoSoYeuCauTiepNhan>
    {
        public override void Configure(EntityTypeBuilder<HoSoYeuCauTiepNhan> builder)
        {
            builder.ToTable(MappingDefaults.HoSoYeuCauTiepNhanTable);

            builder.HasOne(m => m.LoaiHoSoYeuCauTiepNhan)
                .WithMany(u => u.HoSoYeuCauTiepNhans)
                .HasForeignKey(m => m.LoaiHoSoYeuCauTiepNhanId)
                .IsRequired();

            builder.HasOne(m => m.YeuCauTiepNhan)
                .WithMany(u => u.HoSoYeuCauTiepNhans)
                .HasForeignKey(m => m.YeuCauTiepNhanId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}