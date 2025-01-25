using Camino.Core.Domain.Entities.CongTyUuDais;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CongTyUuDaiMapping
{
    public class CongTyUuDaiMap : CaminoEntityTypeConfiguration<CongTyUuDai>
    {
        public override void Configure(EntityTypeBuilder<CongTyUuDai> builder)
        {
            builder.ToTable(MappingDefaults.CongTyUuDaiTable);
            base.Configure(builder);
        }
    }
}