using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class YeuCauTiepNhanLichSuKiemTraTheBHYTMap : CaminoEntityTypeConfiguration<YeuCauTiepNhanLichSuKiemTraTheBHYT>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTiepNhanLichSuKiemTraTheBHYT> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTiepNhanLichSuKiemTraTheBHYTTable);
            builder.HasOne(m => m.YeuCauTiepNhan)
                .WithMany(u => u.YeuCauTiepNhanLichSuKiemTraTheBHYTs)
                .HasForeignKey(m => m.YeuCauTiepNhanId)
                .IsRequired();
        }
    }
}