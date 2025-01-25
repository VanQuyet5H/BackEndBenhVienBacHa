using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DoiTuongUuDais
{
    public class DoiTuongUuDaiMap : CaminoEntityTypeConfiguration<DoiTuongUuDai>
    {
        public override void Configure(EntityTypeBuilder<DoiTuongUuDai> builder)
        {
            builder.ToTable(MappingDefaults.DoiTuongUuDaiTable);
            base.Configure(builder);
        }
    }
}