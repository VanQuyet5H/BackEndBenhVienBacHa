using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class YeuCauTiepNhanLichSuTrangThaiMap : CaminoEntityTypeConfiguration<YeuCauTiepNhanLichSuTrangThai>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTiepNhanLichSuTrangThai> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTiepNhanLichSuTrangThaiTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.YeuCauTiepNhanLichSuTrangThais)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            base.Configure(builder);
        }
    }
}
