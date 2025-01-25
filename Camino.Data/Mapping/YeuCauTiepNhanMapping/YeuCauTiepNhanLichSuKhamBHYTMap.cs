using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class YeuCauTiepNhanLichSuKhamBHYTMap : CaminoEntityTypeConfiguration<YeuCauTiepNhanLichSuKhamBHYT>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTiepNhanLichSuKhamBHYT> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTiepNhanLichSuKhamBHYTTable);
            builder.HasOne(m => m.YeuCauTiepNhan)
                .WithMany(u => u.YeuCauTiepNhanLichSuKhamBHYT)
                .HasForeignKey(m => m.YeuCauTiepNhanId)
                .IsRequired();
        }
    }
}